using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Button[] _buttons;

    private void OnEnable()
    {
        _buttons[0].onClick.AddListener(() => { GameManager.Instance.RestartBtn(); });
        _buttons[1].onClick.AddListener(() => { GameManager.Instance.ExitBtn(); });
    }

    public void UpdateScore(string score)
    {
        _scoreText.text  = score;
    }
}
