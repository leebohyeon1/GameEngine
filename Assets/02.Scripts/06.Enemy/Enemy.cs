using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Animator _animator;

    protected Transform _target;
    protected EnergyGenerator _energyGenerator;

    [SerializeField] protected Transform _attackPoint;

    [SerializeField] protected int _maxHp;
    protected int _curHp;
    
    [SerializeField] protected float _moveSpeed;


    [SerializeField] protected float _attackRange;
    
    [SerializeField] protected float _attackSpeed;
    protected float _attackTimer;
    protected bool _isAttack = false;
    protected bool _canMove = true;
    protected bool _isDead = false; // 사망 여부를 확인하는 변수 추가

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
            _agent.velocity = Vector3.zero; // 에이전트의 속도를 0으로 설정하여 즉시 멈춤
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
            return; // 이미 죽은 상태라면 함수 종료

        _canMove = false;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        _curHp -= damage;

        if (_curHp <= 0)
        {
            _isDead = true; // 사망 상태로 변경
            _animator.SetTrigger("Die");
            StartCoroutine(Die());
        }
        else
        {

            _animator.SetTrigger("GetHit");

        }
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

}
