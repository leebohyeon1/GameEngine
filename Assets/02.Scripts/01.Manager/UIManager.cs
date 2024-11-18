using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject _itemSelectUI;

    public void SetItemSelectUI(bool isOn)
    {
        _itemSelectUI.SetActive(isOn);
    }
}
