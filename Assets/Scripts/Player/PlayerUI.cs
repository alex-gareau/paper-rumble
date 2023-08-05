using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _coinsLabel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStatus.UpdateCoin += UpdateCoinCounter;
    }

    private void UpdateCoinCounter(int amount)
    {
        _coinsLabel.text = amount.ToString();
    }


}
