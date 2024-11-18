using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricWall : BuildObject
{
    [SerializeField] private Transform electric;

    [SerializeField] private int _damagePerSec;
    [SerializeField] private LayerMask _enemyLayer;

    private float _timer = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 0.7f)
        {
            _timer = 0f;
            CheckEnemy();
        }
    }

    private void CheckEnemy()
    {
        Collider collider = electric.GetComponent<Collider>();
        Bounds combinedBounds = new Bounds(collider.bounds.center, collider.bounds.extents);


        Collider[] colliders = Physics.OverlapBox(
        combinedBounds.center,
        combinedBounds.extents,
        Quaternion.identity,
        _enemyLayer); // 충돌할 수 있는 레이어를 제외하여 검사


        foreach (Collider enemyColl in colliders)
        {
            Enemy enemy = enemyColl.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(_damagePerSec);
        }
    }
}
