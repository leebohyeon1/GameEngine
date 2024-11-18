using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelectUI : MonoBehaviour
{
    private PlayerController _playerController;

    [SerializeField] private Image[] _itemImages;
 

    private void OnEnable()
    {
        for (int i = 0; i < _itemImages.Length; i++)
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
        Item item = GameManager.Instance.GetItem(index);

        _itemImages[index].color = new Color(1, 1, 1, 0.5f);
        _playerController.ObjectBuilder.SetObject(item.ItemPrefab, item.price);
    }

    public void UnSelectItem(int index)
    {
        _itemImages[index].color = new Color(0, 0, 0, 0.5f);
    }
}


