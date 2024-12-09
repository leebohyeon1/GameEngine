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
    private List<GameObject> _buildObjectList = new List<GameObject>();    

    protected override void Start()
    {
        _curScore = 0;
        UIManager.Instance.UpdateScore((uint)_curScore); // Update the UI

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
    
    public void SetSapwnManager(SpawnManger spawnManger)
    {
        _spawnManger = spawnManger;
    }

    public void SetPlayer(GameObject player)
    {
        _player = player;
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

    #region BuildObject
    
    public void AddObject(GameObject buildObject)
    {
        if (!_buildObjectList.Contains(buildObject))
        {
            _buildObjectList.Add(buildObject);
        }
     
    }

    public void RemoveObject(GameObject buildObject)
    {
        if (_buildObjectList.Contains(buildObject))
        {
            _buildObjectList.Remove(buildObject);
        }
    }

    public List<GameObject> GetObjectList()
    {
        return _buildObjectList;
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

        _buildObjectList.Clear();
        _player.GetComponent<PlayerController>().Die(_spawnManger.PlayerSpawnPos());

        _energyGenerator.gameObject.SetActive(true);
        _energyGenerator.GetComponent<EnergyGenerator>().ResetStat();

        SetMoney(15);

        ChangeGameState(GameState.Play);
    }


    public void RestartBtn()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; 
        SceneManager.LoadScene("02.Game");
        UIManager.Instance.GameOverUIOff();
    }

    public void ExitBtn()
    {
        SceneManager.LoadScene("01.Title");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMoney(15);
        UIManager.Instance.UpdateMoney();
        _curScore = 0;
        UIManager.Instance.UpdateScore((uint)_curScore);

        ChangeGameState(GameState.Play);

        _curLife = _maxLife;
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

[System.Serializable]
public class Item
{
    public GameObject ItemPrefab;
    public int price;
}
