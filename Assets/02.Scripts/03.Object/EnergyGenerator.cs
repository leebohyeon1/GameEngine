using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGenerator : BuildObject
{
    [SerializeField] private Slider _hpBar;

    void Start()
    {
        UpdateHpBar();
        GameManager.Instance.SetEnergyGenerator(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TakeDamage(int damage)
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
