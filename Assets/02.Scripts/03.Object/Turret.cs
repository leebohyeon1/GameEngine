using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BuildObject
{
    [SerializeField] private Transform _turretBody;
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private Transform _firePos;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private LayerMask _enemyLayer;

    private Transform _target = null;

    [Space(20f)]
    [SerializeField] private float _attackRange;
    [SerializeField] private int _attackDamage;
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

        if (curTarget != null)
        {
            Enemy enemyCom = curTarget.GetComponent<Enemy>();

            _target = enemyCom.GetAttackPoint();
        }
    }

    private void LookTarget()
    {
        if (_target == null)
        {
            return;
        }

        RotateTurretBody();

        // ������ ���� ������� ��ǥ�� �ٶ󺸰� ����
        Quaternion gunBarrelTargetRotation = Quaternion.LookRotation(_target.position - _gunBarrel.position);
        _gunBarrel.rotation = Quaternion.Slerp(_gunBarrel.rotation, gunBarrelTargetRotation, Time.deltaTime * _rotationSpeed);
    }


    private void RotateTurretBody()
    {
        // ��ǥ ��ġ�� Y���� �����ϰ� �ͷ� ��ü�� ��ǥ ���� ���
        Vector3 targetPos = new Vector3(_target.position.x, _turretBody.position.y, _target.position.z);

        // ���� ��� (XZ ��鿡����)
        Vector3 directionToTarget = targetPos - _turretBody.position;

        if (directionToTarget != Vector3.zero)
        {
            // ��ǥ ������ Y�� ȸ�� �� ���
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Vector3 currentEuler = _turretBody.localEulerAngles;

            // ���� ���� ȸ������ X, Z ���� �����ϸ� Y�ุ ȸ��
            Quaternion smoothRotation = Quaternion.Euler(currentEuler.x, targetRotation.eulerAngles.y, currentEuler.z);

            // �ε巴�� �����Ͽ� ȸ��
            _turretBody.localRotation = Quaternion.Slerp(_turretBody.localRotation, smoothRotation, Time.deltaTime * _rotationSpeed);
        }
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
