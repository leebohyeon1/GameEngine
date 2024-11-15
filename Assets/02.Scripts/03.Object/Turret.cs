using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform _turretBody;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private Transform _firePos;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private LayerMask _enemyLayer;

    private Transform _target = null;

    [Space(20f)]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _bulletSpeed;


    [Space(10f)]
    [SerializeField] private float _rotationSpeed;


    private float _timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();

        LookTarget();

        Shoot();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);    

    }

    private void FindTarget()
    {
        if (_target != null)
        {
            return;
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, _attackRange, _enemyLayer);
        float distance = Mathf.Infinity;
        Transform curTarget = null;

        foreach (Collider enemy in enemies)
        {
            if(Vector3.Distance(transform.position, enemy.transform.position) < distance)
            {
                curTarget = enemy.transform; ;
            }
        }

        _target = curTarget;
    }

    private void LookTarget()
    {
        if (_target == null)
        {
            return;
        }

        // 목표 위치의 Y축을 무시하고 터렛 몸체의 목표 방향을 계산합니다.
        Vector3 targetPos = new Vector3(_target.position.x, _turretBody.position.y, _target.position.z);

        // 터렛 몸체의 부드러운 회전
        Quaternion turretTargetRotation = Quaternion.LookRotation(targetPos - _turretBody.position);
        _turretBody.rotation = Quaternion.Slerp(_turretBody.rotation, turretTargetRotation, Time.deltaTime * _rotationSpeed);

        // 포신의 부드러운 회전
        Quaternion gunBarrelTargetRotation = Quaternion.LookRotation(_target.position - _gunBarrel.position);
        _gunBarrel.rotation = Quaternion.Slerp(_gunBarrel.rotation, gunBarrelTargetRotation, Time.deltaTime * _rotationSpeed);
    }


    private void Shoot()
    {
        if (_target == null)
        {
            return;
        }

        _timer += Time.deltaTime;
        if(_timer >= 1 / _attackSpeed)
        {
            _timer = 0;

            GameObject bullet = Instantiate(_bullet, _firePos.position, _gunBarrel.rotation);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(_gunBarrel.forward * _bulletSpeed, ForceMode.Impulse);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDamage(_attackDamage);
        }
    }

}
