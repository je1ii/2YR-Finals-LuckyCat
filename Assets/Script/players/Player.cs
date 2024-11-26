using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class Player : MonoBehaviour
{
    public int playerHealth;
    public bool[] playerResistance;
    public bool isPoisoned;

    private int poisonDeathChance;
    private FirstPersonCamera fpc;
    private Rigidbody rb;
    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        fpc = FindObjectOfType<FirstPersonCamera>();
        playerHealth = 3;
        poisonDeathChance = 1;
        isPoisoned = false;
    }

    void Update()
    {
        if(playerHealth == 0)
        {
            Debug.Log("Player Lost");
            // player died ui
            StartCoroutine(PlayerDiedUI());
        }
    }

    // ui cooldown
    IEnumerator PlayerDiedUI()
    {
        yield return new WaitForSeconds(3f);
        gm.playerDiedUI.SetActive(true);
    }

    IEnumerator LostHP()
    {
        gm.lostHPUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        gm.lostHPUI.SetActive(false);
    }

    public void PoisonPlayer()
    {
        isPoisoned = true;
    }

    public void AddPoison()
    {
        poisonDeathChance++;
    }

    public void CheckPlayerResistance()
    {
        for(int i = 0; i < poisonDeathChance; i++)
        {
            int deathIndex = Random.Range(0, playerResistance.Length);
            if(playerResistance[deathIndex] == true)
            {
                i--;
            }
            else
            {
                playerResistance[deathIndex] = true;
            }
        }

        int fate = Random.Range(0, playerResistance.Length);

        if(playerResistance[fate])
        {
            playerHealth--;
            // player lost 1 hp
            StartCoroutine(LostHP());


            if (playerHealth == 0)
            {
                PlayerHasBeenPoisoned();
            }
        }
        else
        {
            Debug.Log("Player alive for now...");
        }
    }

    public async void PlayerHasBeenPoisoned()
    {
        await Task.Delay(1000);
        fpc.FocusDead();

        await Task.Delay(1000);
        rb.isKinematic = false;
        rb.AddForce(0f,0f,-0.5f, ForceMode.Impulse);

        await Task.Delay(2000);
        rb.isKinematic = true;
    }

    public void GunShot()
    {
        playerHealth-=3;
        PlayerHasBeenShot();
    }

    public async void PlayerHasBeenShot()
    {
        this.gameObject.transform.DOPunchRotation(new Vector3(-100f,0f,0f), 0.3f, 10, 0f);
        await Task.Delay(300);
        rb.isKinematic = false;
        rb.AddForce(0f,0f,-0.5f, ForceMode.Impulse);

        await Task.Delay(2000);
        rb.isKinematic = true;
    }
}
