using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Poison1 : MonoBehaviour
{
    public Collider cd;
    private Rigidbody rb;
    private GameManager gm;
    private Player p;
    private Cork1 c;
    private FirstPersonCamera fpc;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        cd = GetComponent<Collider>();
        gm = FindObjectOfType<GameManager>();
        p = FindObjectOfType<Player>();
        c = FindObjectOfType<Cork1>();
        fpc = FindObjectOfType<FirstPersonCamera>();
    }
    public async void UsePoison()
    {
        await UsingPoison();
        gm.ResetRound();
    }

    public async Task UsingPoison()
    {
        if(!fpc.focus2ndPoison)
        {
            fpc.FocusOn2ndPoison();
        }

        gm.playerLost = false;
        fpc.lockCam = true;
        c.OpenBottle();

        await Task.Delay(2000);
        Sequence bottleSequence = DOTween.Sequence();
        bottleSequence.Append(transform.DOMove(new Vector3(-5.25f, 0.9f, 2.2f), 0.5f));

        await Task.Delay(600);
        Sequence bottle2Sequence = DOTween.Sequence();
        bottle2Sequence.Append(transform.DOMove(new Vector3(-5.25f, 1.1f, 2.2f), 2f));
        bottle2Sequence.Join(transform.DORotate(new Vector3(35f, 120f, 0f), 2f));


        await Task.Delay(2500);
        fpc.UnFocusOn2ndPoison();
        fpc.lockCam = false;
        rb.isKinematic = false;

        await Task.Delay(500);
        p.AddPoison();
        this.gameObject.SetActive(false);
    }

    public void OnTrigger()
    {
        cd.isTrigger = true;
    }
}
