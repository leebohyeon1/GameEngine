using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObject : MonoBehaviour
{
    [SerializeField] protected int _maxHp;
    protected int _curHp = 0;

    protected ObjectBuilder _builder;


    protected virtual void Awake()
    {
        _curHp = _maxHp;
    }

    public virtual void TakeDamage(int damamge)
    {
        _curHp -= damamge;

        if(_curHp <= 0 )
        {
            _builder.ObjectClones.Remove(gameObject);
            GameManager.Instance.RemoveObject(gameObject);

            Destroy(gameObject);
        }
    }

    public void SetBuilder(ObjectBuilder objectBuilder)
    {
        _builder = objectBuilder;
    }
}
