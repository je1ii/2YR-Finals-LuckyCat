using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cork1 : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    public void OpenBottle()
    {
        Sequence cork = DOTween.Sequence();
        cork.Append(transform.DOLocalMoveY(2f, 1f)).SetEase(Ease.OutSine);
        cork.Join(transform.DOLocalMoveX(-1.0f, 1f)).SetEase(Ease.OutSine);
        cork.Join(transform.DORotate(new Vector3(-70f, -90f, 90f), 0.6f));
        cork.Join(transform.DORotate(new Vector3(-90f, -90f, 90f), 0.6f));
        rb.isKinematic = false;
    }
}
