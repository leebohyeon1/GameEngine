using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectUI : MonoBehaviour
{
    private GameObject _curItem;
    private PlayerController _playerController;

    [SerializeField] private ItemSelection[] _items;
 

    private void OnEnable()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            UnSelectItem(i);
        }
    }

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    public void SelectItem(int index)
    {
        _items[index].ItemImage.color = new Color(1, 1, 1, 0.5f);

        _curItem = _items[index].ItemPrefab;
        _playerController.ObjectBuilder.SetObject(_curItem);
    }

    public void UnSelectItem(int index)
    {
        _items[index].ItemImage.color = new Color(0, 0, 0, 0.5f);
    }
}

[System.Serializable]
public class ItemSelection
{
    public Image ItemImage;
    public GameObject ItemPrefab;
}

