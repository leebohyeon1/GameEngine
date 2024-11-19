using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Play = 0,
    Pause = 1,
    ItemSelect = 2,
    TimeBreakDown = 3,
    GameOver = 4,
}

public class GameManager : Singleton<GameManager>
{
    private Transform _energyGenerator;

    [SerializeField] private GameObject _player;
    [SerializeField] private SpawnManger _spawnManger;

    [SerializeField] private Item[] items;

    [SerializeField] private int _maxLife; // 총 목숨 갯수
    private int _curLife;



    private GameState _curGameState; // 현재 게임 상태
    private uint _curMoney; // 현재 자원
    private float _curScore; // 현재 점수

    private List<Enemy> _enemyList = new List<Enemy>(); // 소환한 적 리스트

    protected override void Start()
    {
        ChangeGameState(GameState.Play);

        SetMoney(15);

        _curLife = _maxLife;
    }

    protected override void Update()
    {
        if(_curGameState == GameState.Play || _curGameState == GameState.ItemSelect)
        {
            UpdateScore();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }


    public void SetEnergyGenerator(Transform energyGenerator)
    {
        _energyGenerator = energyGenerator;
        _spawnManger.SetEnergyGenerator(_energyGenerator);
    }

    #region GameState
    public void ChangeGameState(GameState gameState)
    {
        _curGameState = gameState;

        switch (_curGameState)
        {
            case GameState.Play:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                UIManager.Instance.SetItemSelectUI(false);
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.ItemSelect:
                Time.timeScale = 0.3f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                UIManager.Instance.SetItemSelectUI(true);
                break;
            case GameState.TimeBreakDown:
                Time.timeScale = 0f;

                RestartGame();
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                UIManager.Instance.GameOverUIOn();
                break;
        }
    }

    public GameState GetCurGameState() => _curGameState;

    #endregion

    #region Enemy

    public void AddEnemy(Enemy enemy)
    {
        _enemyList.Add(enemy);
    }

    public void DestroyAllEnemies()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            if (_enemyList[i] != null)
            {
                Destroy(_enemyList[i].gameObject);
            }
        }
        _enemyList.Clear();
    }

    #endregion

    #region Money

    public uint GetMoney() => _curMoney;

    public void DecreaseMoney(int amount)
    {
        _curMoney -= (uint)amount;
        UIManager.Instance.UpdateMoney();
    }

    public void IncreaseMoney(int amount)
    {
        _curMoney += (uint)amount;
        UIManager.Instance.UpdateMoney();
    }

    public void SetMoney(int amount)
    {
        _curMoney = (uint)amount;
        UIManager.Instance.UpdateMoney();
    }
    #endregion

    public Item GetItem(int index)
    {
        return items[index];    
    }

    #region Score

    public void UpdateScore()
    {
        _curScore += Time.deltaTime;
        UIManager.Instance.UpdateScore((uint)_curScore);     
    }

    public void IncreaseScore(int score)
    {
        _curScore += score;
        UIManager.Instance.UpdateScore((uint)_curScore);
    }

    #endregion

    private void RestartGame()
    {
        _curLife--;

        DestroyAllEnemies();

        if (_curLife <= 0)
        {
            ChangeGameState(GameState.GameOver);
            return;
        }

        _player.GetComponent<PlayerController>().Die(_spawnManger.PlayerSpawnPos());

        _energyGenerator.gameObject.SetActive(true);
        _energyGenerator.GetComponent<EnergyGenerator>().ResetStat();

        SetMoney(15);

        ChangeGameState(GameState.Play);
    }


    public void RestartBtn()
    {
        SceneManager.LoadScene("02.Game");
        UIManager.Instance.GameOverUIOff();

        ChangeGameState(GameState.Play);
    }

    public void ExitBtn()
    {
        SceneManager.LoadScene("01.Title");
    }
}

[System.Serializable]
public class Item
{
    public GameObject ItemPrefab;
    public int price;
}
