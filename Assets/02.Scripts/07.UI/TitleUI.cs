using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button _startBtn;
    // Start is called before the first frame update
    void Start()
    {
        _startBtn.onClick.AddListener(() => { StartBtn(); });
    }

    private void StartBtn()
    {
        SceneManager.LoadScene("02.Game");
    }
}
