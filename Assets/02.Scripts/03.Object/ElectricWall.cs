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
        if(_timer > 1f)
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
        _enemyLayer); // �浹�� �� �ִ� ���̾ �����Ͽ� �˻�


        foreach (Collider enemyColl in colliders)
        {
            Enemy enemy = enemyColl.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(_damagePerSec);
        }
    }
}
