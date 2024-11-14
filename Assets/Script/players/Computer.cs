using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    public int computerHealth;

    void Start()
    {
        computerHealth = 2;
    }

    void Update()
    {
        if(computerHealth == 0)
        {
            Debug.Log("Computer Lost");
        }
    }
}
