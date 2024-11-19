using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGenerator : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private float _maxHp;
    
    private float _curHp;

    // Start is called before the first frame update
    void Start()
    {
        _curHp = _maxHp;
        UpdateHpBar();
        GameManager.Instance.SetEnergyGenerator(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        _curHp -= damage;

        if (_curHp <= 0)
        {
            gameObject.SetActive(false);
            GameManager.Instance.ChangeGameState(GameState.TimeBreakDown);
            
        }

        UpdateHpBar();
    }

    public void ResetStat()
    {
        _curHp = _maxHp;
        UpdateHpBar();
    }

    private void UpdateHpBar()
    {
        _hpBar.value = (float)_curHp / _maxHp;
    }
}
