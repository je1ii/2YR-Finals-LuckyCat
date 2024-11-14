using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> decoyDeck = new List<GameObject>();
    public int decoyIndex;

    void Start()
    {
        decoyIndex = decoyDeck.Count - 1;
    }

    public void TakeCardOnDeck()
    {
        decoyDeck[decoyIndex].gameObject.SetActive(false);
        decoyIndex--;
    }

    public void PutCardOnDeck()
    {
        decoyIndex++;
        decoyDeck[decoyIndex].gameObject.SetActive(true);
    }
}
