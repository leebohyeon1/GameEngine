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

    private GameState _curGameState;
    private Transform _energyGenerator;

    // Start is called before the first frame update
    protected override void Start()
    {
        ChangeGameState(GameState.Play);
        //StartCoroutine(LoadEnvironmentScene());
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

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
                
                break;
        }
    }

    public GameState GetCurGameState() => _curGameState;


    private IEnumerator LoadEnvironmentScene()
    {
        ChangeGameState(GameState.Pause);
        // 씬을 비동기적으로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("EnvironmentScene", LoadSceneMode.Additive);

        // 씬이 로드될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        ChangeGameState(GameState.Play);
    }

    public void SetEnergyGenerator(Transform energyGenerator)
    {
        _energyGenerator = energyGenerator;
        _spawnManger.SetEnergyGenerator(_energyGenerator);
    }
}
