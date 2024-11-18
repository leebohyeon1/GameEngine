using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _damage;
    
    private float _timer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DestroySelf();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(_damage);
        }
        Destroy(gameObject);
    }

    private void DestroySelf()
    {
        _timer += Time.deltaTime;
        if( _timer > 6f )
        {
            Destroy(gameObject);
        }
    }


    public void SetDamage(int damage)
    {
        _damage = damage;
    }
}
