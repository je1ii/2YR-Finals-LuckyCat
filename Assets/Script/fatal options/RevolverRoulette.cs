using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class RevolverRoulette : MonoBehaviour
{
    public bool[] magazine;
    public int bulletIndex;
    public int fateIndex;
    public bool hasBeenShot;

    private GameManager gm;
    private Player p;
    private FirstPersonCamera fpc;
    private Rigidbody rb;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        gm = FindObjectOfType<GameManager>();
        p = FindObjectOfType<Player>();
        fpc = FindObjectOfType<FirstPersonCamera>();

        hasBeenShot = false;

        ReloadMagazine();
    }

    public async void UseGun()
    {
        await UsingGun();
        gm.ResetRound();
    }

    public async Task UsingGun()
    {
        if(fpc.focusRevolver)
        {
            fpc.UnFocusOnGun();
        }
        
        gm.playerLost = false;
        fpc.lockCam = true;

        Sequence getGunSequence = DOTween.Sequence();
        getGunSequence.Append(transform.DOMove(new Vector3(-5.35f, 1.1f, 2.45f), 1f));
        getGunSequence.Join(transform.DORotate(new Vector3(-1.8f, -40f, 90f), 1f));

        await Task.Delay(3000);
        PullTrigger();

        if(hasBeenShot)
        {
            fpc.RemoveDampening();
            magazine[bulletIndex] = false;

            await Task.Delay(1000);
            p.GunShot();
            rb.isKinematic = false;

            await Task.Delay(100);
            fpc.FocusDead();
        }
        else
        {
            Sequence putBackGunSequence = DOTween.Sequence();
            putBackGunSequence.Append(transform.DOMove(new Vector3(-6f, 0.776f, 2.15f), 1f));
            putBackGunSequence.Join(transform.DORotate(new Vector3(0f, 180f, 0f), 1f));
        }
        
        fpc.lockCam = false;
    }

    public void PullTrigger()
    {
        fateIndex = Random.Range(0, magazine.Length);

        if(magazine[fateIndex])
        {
            hasBeenShot = true;
        }
        else
        {
            Debug.Log("You Alive");
        }
    }

    public void ReloadMagazine()
    {
        for(int i = 0; i < 3; i++)
        {
            bulletIndex = Random.Range(0, magazine.Length);
            if(magazine[bulletIndex] == true)
            {
                i--;
            }
            else
            {
                magazine[bulletIndex] = true;
            }
        }
    }
}