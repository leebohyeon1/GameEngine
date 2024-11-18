using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Play = 0,
    Pause = 1,
    ItemSelect =2,
    GameOver = 3,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SpawnManger _spawnManger;

    [SerializeField] private Item[] items;

    private GameState _curGameState; // 현재 게임 상태
    private Transform _energyGenerator;

    private uint _curMoney;
    private float _curScore;


    private List<Enemy> _enemyList = new List<Enemy>();



    // Start is called before the first frame update
    protected override void Start()
    {
        ChangeGameState(GameState.Play);

        IncreaseMoney(15);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(_curGameState == GameState.Play || _curGameState == GameState.ItemSelect)
        {
            UpdateScore();
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
            case GameState.GameOver:

                DestroyAllEnemies();
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
}

[System.Serializable]
public class Item
{
    public GameObject ItemPrefab;
    public int price;
}
