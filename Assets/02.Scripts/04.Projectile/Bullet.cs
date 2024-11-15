using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _damage;
    
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
            Destroy(other.gameObject);
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


    public void SetDamage(float damage)
    {
        _damage = damage;
    }
}
