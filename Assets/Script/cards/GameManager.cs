using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{    

    // controls: press "SPACE" to check cards, press "R" to draw another card, press "Q" to call computer's bluff(NOT YET CODED),
    //           press "W" to end the round, hold "A" to look at gun, hold "D" to look at poison, press "E" to interact with gun and poison


    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    // player hand
    public List<Card> playerHand = new List<Card>();
    public Transform[] playerSlots;
    public bool[] availPlayerSlots;
    public int playerTotalHand;
    public bool playerLost = false;

    // computer hand
    public List<Card> compHand = new List<Card>();
    public Transform[] compSlots;
    public bool[] availCompSlots;
    public int compTotalHand;
    public bool compLost = false;

    public float afterRoundTimer;
    public bool startTimer;
    private Poison poison;
    private Poison1 poison1;
    private RevolverRoulette gun;
    private Deck decoy;
    private DiscardPile dp;
    private Player p;

    void Awake()
    {
        decoy = FindObjectOfType<Deck>();
        dp = FindObjectOfType<DiscardPile>();
        poison = FindObjectOfType<Poison>();
        poison1 = FindObjectOfType<Poison1>();
        gun = FindObjectOfType<RevolverRoulette>();
        p = FindObjectOfType<Player>();
    }

    async void Start()
    {
        afterRoundTimer = 30f;
        startTimer = false;

        // wait for two seconds to start round
        await Task.Delay(2000);
        StartRound();
    }

    void Update()
    {
        // playerScoreText = playerTotalHand; insert ui text here

        if(Input.GetKeyDown(KeyCode.R) && playerHand.Count > 1)
        {
            if(playerHand.Count == 3)
            {
                // put ui here to let the player know they cant draw anymore card
            }
            else
            {
                ReDrawForPlayer();
            }
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            AfterRoundStatus();
        }

        if(startTimer)
        {
            // set start timer ui here 

            afterRoundTimer -= Time.deltaTime;

            if(afterRoundTimer <= 0.0f)
            {
                // set end timer ui here 
            }
        }
    }

    public void StartRound()
    {
        DrawForEveryone();
    }

    public async void AfterRoundStatus()
    {
        GetScoreForEveryone();
        CheckWinner();

        //startTimer = true;
        if(playerLost) 
        {
            // player lost
            Debug.Log("Waiting for player...");
            await WaitingForPlayer();

            // if player did not choose and ran out of time
            if(playerLost)
            {
                int fate = Random.Range(0, 10);

                if(fate > 5)
                {
                    if(poison.gameObject.activeSelf == false)
                    {
                        poison1.UsePoison();
                    }
                    else
                    {
                        poison.UsePoison();
                    }
                }
                else
                {
                    gun.UseGun();
                }
            }
        }
        else if(compLost)
        {
            // computer lost
            Debug.Log("Waiting for computer...");
            await WaitingForComputer();
            ResetRound();
        }
        else
        {
            // draw
            Debug.Log("Draw. The next round will start in a few.");
            await Task.Delay(3000);
            ResetRound();
        }

        //ResetRound();
    }   

    public async Task WaitingForPlayer()
    {
        // will wait player to choose which option they choose 
        // but will auto choose if time ends and player didnt choose

        // set ui here to let the player know

        await Task.Delay(5000); //TEMP TIMER
        //await Task.Delay(30000);
    }

    public async Task WaitingForComputer()
    {
        // will wait computer to choose which option they choose

        // set ui here to let the player know

        await Task.Delay(5000); //TEMP TIMER
        //await Task.Delay(30000);
    }

    public async void ResetRound()
    {
        playerTotalHand = 0;
        playerLost = false;

        compTotalHand = 0;
        compLost = false;

        startTimer = false;
        afterRoundTimer = 30f;

        if(decoy.decoyIndex < 8)
        {
            await ShuffleAllCards();
        }

        if(p.isPoisoned)
        {
            p.CheckPlayerResistance();

        }

        // second delay before starting round again
        await Task.Delay(1000);
        StartRound();
    }

    public void CheckWinner()
    {
        int pScore = playerTotalHand - 9;
        int cScore = compTotalHand - 9;

        Debug.Log(playerTotalHand);
        Debug.Log(compTotalHand);

        if(Mathf.Abs(cScore) > Mathf.Abs(pScore))
        {
            Debug.Log("Player Wins");
            compLost = true;
        }
        else if(Mathf.Abs(pScore) > Mathf.Abs(cScore))
        {
            Debug.Log("Computer Wins");
            playerLost = true;
        }
        else
        {
            Debug.Log("NO ONE WINS :)");
        }
    }

    public async Task ShuffleAllCards()
    {
        if(discardPile.Count >= 1)
        {
            foreach(Card card in discardPile)
            {
                deck.Add(card);
                card.gameObject.SetActive(true);
                card.MoveBackToDeck();

                await Task.Delay(200);
                decoy.PutCardOnDeck();

                await Task.Delay(200);
                card.gameObject.SetActive(false);
            }
            discardPile.Clear();
        }
    }

    public async void DrawCard(List<Card> inHand, Transform[] cardSlots, bool[] availCardSlots, bool isPlayer)
    {
        if(deck.Count >= 1)
        {
            for(int i = 0; i < 2; i++)
            {
                Card randCard = deck[Random.Range(0, deck.Count)];
                randCard.gameObject.SetActive(true);

                // move card to decoy position before moving to a card slot
                randCard.transform.position = decoy.decoyDeck[decoy.decoyIndex].transform.position;
                decoy.TakeCardOnDeck();
                randCard.MoveToHand(cardSlots, i);

                // assign card if its in the players hand or not
                if(isPlayer)
                {
                    randCard.isPlayerHand = true;
                }
                else
                {
                    randCard.isPlayerHand = false;
                }

                randCard.handIndex = i;
                randCard.hasBeenPlayed = false;
                randCard.isReDraw = false;
                randCard.isCardsDown = true;
                availCardSlots[i] = false;
                inHand.Add(randCard);
                deck.Remove(randCard);

                await Task.Delay(500);
            }
        }
    }

    public void ReDrawCard(List<Card> inHand, Transform[] cardSlots, bool[] availCardSlots, bool isPlayer)
    {
        if(deck.Count >= 1)
        {
            for(int i = 2; i < 3; i++)
            {
                Card randCard = deck[Random.Range(0, deck.Count)];
                randCard.gameObject.SetActive(true);

                // move card to decoy position before moving to a card slot
                randCard.transform.position = decoy.decoyDeck[decoy.decoyIndex].transform.position;
                decoy.TakeCardOnDeck();
                randCard.MoveToHand(cardSlots, i);

                // assign card if its in the players hand or not
                if(isPlayer)
                {
                    randCard.isPlayerHand = true;
                }
                else
                {
                    randCard.isPlayerHand = false;
                }

                randCard.handIndex = i;
                randCard.isReDraw = true;
                randCard.hasBeenPlayed = false;
                randCard.isCardsDown = true;
                
                // will put up new card if all cards are up
                if(playerHand[0].isCardsDown == false && isPlayer)
                {
                    randCard.CheckCards();
                }

                availCardSlots[i] = false;
                inHand.Add(randCard);
                deck.Remove(randCard);
            }
        }
    }

    public int GetTotalCardValue(List<Card> cards)
    {   
        int[] value = {0, 0, 0};
        int totalValue;
        for(int i = 0; i < cards.Count; i++)
        {
            switch(cards[i].gameObject.tag)
            {
                case "Ace":
                    value[i] = 1;
                    break;
                case "Two":
                    value[i] = 2;
                    break;
                case "Three":
                    value[i] = 3;
                    break;
                case "Four":
                    value[i] = 4;
                    break;
                case "Five":
                    value[i] = 5;
                    break;
                case "Six":
                    value[i] = 6;
                    break;
                case "Seven":
                    value[i] = 7;
                    break;
                case "Eight":
                    value[i] = 8;
                    break;
                case "Nine":
                    value[i] = 9;
                    break;
                case "Ten&Face":
                    value[i] = 0;
                    break;
                default:
                    break;
            }
        }
        totalValue = value[0] + value[1] + value[2];

        if(totalValue > 9 && totalValue < 20)
        {
            totalValue-=10;
        }
        else if(totalValue > 19 && totalValue < 30)
        {
            totalValue-=20;
        }
        else if(totalValue > 30)
        {
            totalValue-=30;
        }

        return totalValue;
    }

    public void GetScoreForEveryone()
    {
        playerTotalHand = GetTotalCardValue(playerHand);
        compTotalHand = GetTotalCardValue(compHand);
    }

    public async void DrawForEveryone()
    {
        DrawCard(playerHand,playerSlots,availPlayerSlots,true);

        await Task.Delay(1500);
        DrawCard(compHand,compSlots,availCompSlots,false);
    }

    public void ReDrawForPlayer()
    {
        ReDrawCard(playerHand,playerSlots,availPlayerSlots,true);
    }

    public void ReDrawForComputer()
    {
        ReDrawCard(compHand,compSlots,availCompSlots,false);
    }
}
