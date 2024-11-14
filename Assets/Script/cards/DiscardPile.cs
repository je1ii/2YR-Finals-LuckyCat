using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public List<GameObject> pile = new List<GameObject>();
    public int pileIndex;
    
    void Start()
    {
        pileIndex = 0;
    }

    public void PutCardOnPile()
    {
        pile[pileIndex].gameObject.SetActive(true);
        pileIndex++;
    }

    public void TakeCardOnPile()
    {
        pileIndex--;
        pile[pileIndex].gameObject.SetActive(false);
    }
}
