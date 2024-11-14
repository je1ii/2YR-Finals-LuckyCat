using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class Card : MonoBehaviour
{
    public int handIndex;
    public bool hasBeenPlayed,isPlayerHand,isReDraw;

    public bool isCardsDown;
    private GameManager gm;
    private DiscardPile dp;
    private Deck decoy;
    private FirstPersonCamera fpc;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        dp = FindObjectOfType<DiscardPile>();
        decoy = FindObjectOfType<Deck>();
        fpc = FindObjectOfType<FirstPersonCamera>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isPlayerHand)
        {
            CheckCards();
            gm.GetScoreForEveryone();
        }

        if(gm.playerHand.Count == 3 && !isCardsDown)
        {
            gm.GetScoreForEveryone();
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            SetUpNextRound();
        }

    }

    public void MoveToHand(Transform[] slots, int index)
    {
        Sequence cardSequence = DOTween.Sequence();
        cardSequence.Append(transform.DOMove(slots[index].position, 0.5f));
    }

    public void SetUpNextRound()
    {
        if(hasBeenPlayed == false)
        {
            if(!isCardsDown)
            {
                CheckCards();
            }

            hasBeenPlayed = true;
            gm.availPlayerSlots[handIndex] = true;
            gm.availCompSlots[handIndex] = true;
            
            if(isPlayerHand)
            {
                gm.playerHand.Remove(this);
            }
            else
            {
                gm.compHand.Remove(this);
            }

            MoveToDiscardPile();
        }
    }

    public async void MoveToDiscardPile()
    {
        gm.discardPile.Add(this);
        Sequence cardSequence = DOTween.Sequence();
        cardSequence.Append(transform.DOMove(dp.pile[dp.pileIndex].transform.position, 0.5f)).SetEase(Ease.OutSine);

        await Task.Delay(500);
        dp.PutCardOnPile();
        this.gameObject.SetActive(false);
    }

    public async void MoveBackToDeck()
    {
        dp.TakeCardOnPile();
        this.gameObject.transform.position = dp.pile[dp.pileIndex].transform.position;

        await Task.Delay(100);
        Sequence card1Sequence = DOTween.Sequence();
        card1Sequence.Append(transform.DOMove(new Vector3(-6f, 0.9f, 2.15f), 0.2f)).SetEase(Ease.OutCirc);

        await Task.Delay(200);
        Sequence card2Sequence = DOTween.Sequence();
        card2Sequence.Append(transform.DOMove(decoy.decoyDeck[decoy.decoyIndex].transform.position, 0.2f)).SetEase(Ease.InCirc);
    }

    public void CheckCards()
    {
        if(!hasBeenPlayed)
        {
            Sequence cardSequence = DOTween.Sequence();
            if(isCardsDown)
            {
                if(!fpc.focusCards)
                {
                    fpc.FocusOnCards();
                }

                if(isReDraw)
                {
                    cardSequence.Append(transform.DOMoveX(-5.45f, 0.6f)).SetEase(Ease.InSine);
                    cardSequence.Join(transform.DOMoveY(1.0f, 0.6f)).SetEase(Ease.InSine);
                    cardSequence.Join(transform.DORotate(new Vector3(-90, 180, 90), 0.6f));
                }
                else
                {
                    cardSequence.Append(transform.DOMoveX(-5.35f, 0.65f)).SetEase(Ease.InSine);
                    cardSequence.Join(transform.DOMoveY(0.9f, 0.65f)).SetEase(Ease.InSine);
                    cardSequence.Join(transform.DORotate(new Vector3(-90, 180, 90), 0.65f));
                }
                
                isCardsDown = false;
            }
            else
            {
                if(fpc.focusCards)
                {
                    fpc.UnFocusOnCards();
                }

                if(isReDraw)
                {
                    cardSequence.Append(transform.DOMoveX(-5.65f, 0.6f)).SetEase(Ease.OutSine);
                    cardSequence.Join(transform.DOMoveY(0.776f, 0.6f)).SetEase(Ease.OutSine);
                    cardSequence.Join(transform.DORotate(new Vector3(0, 90, 180), 0.6f));
                }
                else
                {
                    cardSequence.Append(transform.DOMoveX(-5.5f, 0.65f)).SetEase(Ease.OutSine);
                    cardSequence.Join(transform.DOMoveY(0.776f, 0.65f)).SetEase(Ease.OutSine);
                    cardSequence.Join(transform.DORotate(new Vector3(0, 90, 180), 0.65f));
                }

                isCardsDown = true;
            }
        }
    }
}
