using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Play = 0,
    Pause = 1,
    ItemSelect =2,
    GameOver = 3,
}

public class GameManager : Singleton<GameManager>
{
    private GameState _curGameState;

    // Start is called before the first frame update
    protected override void Start()
    {
        ChangeGameState(GameState.Play);
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

}
