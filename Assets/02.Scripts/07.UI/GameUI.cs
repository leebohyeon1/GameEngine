using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _scoreText;

    public void UpdateMoney(string money)
    {
        _moneyText.text = money;
    }

    public void UpdateScore(string score)
    {
        _scoreText.text = score;
    }
}
