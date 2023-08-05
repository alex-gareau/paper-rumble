using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{

    public int Coin {get; set;}

    public static event Action<int> UpdateCoin;
    public static event Action<int> UpdateHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCoin(int amount)
    {
        Coin += amount;

        UpdateCoin?.Invoke(Coin);
    }
}
