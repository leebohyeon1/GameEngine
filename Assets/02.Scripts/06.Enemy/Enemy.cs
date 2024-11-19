using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Animator _animator;

    protected Transform _target;
    protected EnergyGenerator _energyGenerator;

    [SerializeField] protected Slider _hpBar;
    [SerializeField] protected Transform _attackPoint;


    [Space(20f)]
    [SerializeField] protected int _maxHp;
    protected int _curHp;
    
    [SerializeField] protected float _moveSpeed;


    [SerializeField] protected float _attackRange;
    
    [SerializeField] protected float _attackSpeed;
    protected float _attackTimer;
    protected bool _isAttack = false;
    protected bool _canMove = true;
    protected bool _isDead = false; // ��� ���θ� Ȯ���ϴ� ���� �߰�

    [SerializeField] protected float _attackDamage;

    [SerializeField] protected int _dropMoney;
    [SerializeField] protected int _dropScore;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _agent.stoppingDistance = _attackRange;

        _curHp = _maxHp;
        UpdateHpBar();
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    protected virtual void Update()
    {
        EnemyAction();
    }

    protected virtual void EnemyAction()
    {
        if (_target == null || !_canMove)
        {
            return;
        }

        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= _attackRange)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero; // ������Ʈ�� �ӵ��� 0���� �����Ͽ� ��� ����
            _animator.SetBool("Move", false);

            _attackTimer += Time.deltaTime;

            if (_attackTimer >= 1 / _attackSpeed && !_isAttack)
            {
                _isAttack = true;
                _animator.SetTrigger("Attack");
                _animator.SetInteger("AttackIndex", Random.Range(0, 2));

                _attackTimer = 0f;
                _energyGenerator.TakeDamage(_attackDamage);
            }
        }
        else
        {
            _agent.isStopped = false;
            _animator.SetBool("Move", true);
            _agent.SetDestination(_target.position);
        }
    }


    public virtual void TakeDamage(int damage)
    {
        if (_isDead)
            return; // �̹� ���� ���¶�� �Լ� ����

        _canMove = false;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        _curHp -= damage;

        if (_curHp <= 0)
        {
            _isDead = true; // ��� ���·� ����
            _animator.SetTrigger("Die");
            StartCoroutine(Die());
        }
        else
        {

            _animator.SetTrigger("GetHit");

        }

        UpdateHpBar();
    }

    public virtual void AttackEnd()
    {
        _isAttack = false;
    }

    public virtual void OnMove()
    {
        _canMove = true;
        _agent.isStopped = false;
        
    }

    public virtual IEnumerator Die()
    {
        yield return new WaitForSeconds(1.4f);
        GameManager.Instance.IncreaseMoney(_dropMoney);
        GameManager.Instance.IncreaseScore(_dropScore);
        Destroy(gameObject);
    }

    public virtual void SetTarget(Transform target)
    {
        _target = target;
        _energyGenerator = _target.GetComponent<EnergyGenerator>();
    }

    public virtual Transform GetAttackPoint()
    {
        return _attackPoint;
    }

    protected virtual void UpdateHpBar()
    {
        _hpBar.value = (float)_curHp / _maxHp;
    }
}
