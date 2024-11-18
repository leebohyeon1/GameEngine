using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : MonoBehaviour
{
    [SerializeField] private float _maxHp;
    private float _curHp;

    // Start is called before the first frame update
    void Start()
    {
        _curHp = _maxHp;
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
    }

    public void ResetStat()
    {
        _curHp = _maxHp;
    }
}
