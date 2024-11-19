using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] ItemSelectUI _itemSelectUI;
    [SerializeField] GameUI _gameUI;
    [SerializeField] GameOverUI _gameOverUI;

    public void SetItemSelectUI(bool isOn)
    {
        _itemSelectUI.gameObject.SetActive(isOn);
    }

    public void UpdateMoney()
    {
        _gameUI.UpdateMoney(GameManager.Instance.GetMoney().ToString());
    }

    public void UpdateScore(uint score)
    {
        _gameUI.UpdateScore(score.ToString());
        _gameOverUI.UpdateScore(score.ToString());
    }

    public void GameOverUIOn()
    {
        _gameOverUI.gameObject.SetActive(true);
    }

    public void GameOverUIOff()
    {
        _gameOverUI.gameObject.SetActive(false);
    }
}
