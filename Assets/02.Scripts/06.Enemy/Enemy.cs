using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    protected NavMeshAgent _agent;
    protected Animator _animator;

    protected BuildObject _subTarget;
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
    protected bool _isDead = false; // 사망 여부를 확인하는 변수 추가

    [SerializeField] protected int _attackDamage;

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
        if (_energyGenerator == null || !_canMove)
        {
            return;
        }

        if( (_subTarget != null && !IsPathValid(_subTarget.transform.position)) || (_subTarget == null && !IsPathValid(_energyGenerator.transform.position)))
        {
            FindClosestBuildObject();
        }

        EnemyAction();
    }

    protected bool IsPathValid(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(targetPosition, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    protected virtual void EnemyAction()
    {
        if (_energyGenerator == null || !_canMove)
        {
            return;
        }

        // 서브 타겟이 있으면 서브 타겟으로 이동
        if (_subTarget != null)
        {
            MoveToSubTarget();
            return;
        }

        // 서브 타겟이 없으면 일반 타겟으로 이동
        if (_energyGenerator != null)
        {
            MoveToTarget();
        }
    }

    protected void MoveToSubTarget()
    {
        if (_subTarget == null)
        {
            return;
        }

        float distance = Vector3.Distance(_subTarget.transform.position, transform.position);

        if (distance <= _attackRange)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _animator.SetBool("Move", false);

            _attackTimer += Time.deltaTime;

            if (_attackTimer >= 1 / _attackSpeed && !_isAttack)
            {
                _isAttack = true;
                _animator.SetTrigger("Attack");
                _animator.SetInteger("AttackIndex", Random.Range(0, 2));

                _attackTimer = 0f;
                _subTarget?.TakeDamage(_attackDamage);
            }
        }
        else
        {
            _agent.isStopped = false;
            _animator.SetBool("Move", true);
            _agent.SetDestination(_subTarget.transform.position);
        }

        // 서브 타겟이 사라지거나 제거되었는지 확인
        if (_subTarget == null)
        {
            _subTarget = null;
        }
    }

    protected void MoveToTarget()
    {
        if (_energyGenerator == null)
        {
            return;
        }

        float distance = Vector3.Distance(_energyGenerator.transform.position, transform.position);

        if (distance <= _attackRange)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _animator.SetBool("Move", false);

            _attackTimer += Time.deltaTime;

            if (_attackTimer >= 1 / _attackSpeed && !_isAttack)
            {
                _isAttack = true;
                _animator.SetTrigger("Attack");
                _animator.SetInteger("AttackIndex", Random.Range(0, 2));

                _attackTimer = 0f;
                _energyGenerator?.TakeDamage(_attackDamage);
            }
        }
        else
        {
            _agent.isStopped = false;
            _animator.SetBool("Move", true);
            _agent.SetDestination(_energyGenerator.transform.position);
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
        _energyGenerator = target.GetComponent<EnergyGenerator>();
    }

    public virtual void SetSubTarget(Transform target)
    {
        _subTarget = target.GetComponent<BuildObject>();
    }
    
    public virtual Transform GetAttackPoint()
    {
        return _attackPoint;
    }

    protected virtual void UpdateHpBar()
    {
        _hpBar.value = (float)_curHp / _maxHp;
    }

    protected void FindClosestBuildObject()
    {
        float minDistance = Mathf.Infinity;
        List<GameObject> objList = GameManager.Instance.GetObjectList();

        if (objList == null || objList.Count == 0)
        {
            return;
        }

        foreach (GameObject obj in objList)
        {
            if(obj == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;

                SetSubTarget(obj.transform);
            }

        }
        
    }


}
