using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManger : MonoBehaviour
{
    [SerializeField] private Transform _energyGenerator;
    [SerializeField] private Transform _playerStartPosition;

    [Space(20f)]
    [SerializeField] private float _spawnRadius;

    [SerializeField] private float _spawnInterval;
    private float _timer = 0f;

    [SerializeField] private GameObject[] enemyObject;

    private void Start()
    {
        GameManager.Instance.SetSapwnManager(this);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > _spawnInterval)
        {
            _timer = 0f;
            int i = Random.Range(0, enemyObject.Length);
            Spawn(enemyObject[i]);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_energyGenerator.position, _spawnRadius); 
    }

    public void SetEnergyGenerator(Transform energyGenerator)
    {
        _energyGenerator = energyGenerator;
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 randomDirection =  Random.insideUnitCircle.normalized * _spawnRadius;
        Vector3 spawnPosition = new Vector3(randomDirection.x, 1f, randomDirection.y);

        spawnPosition += _energyGenerator.position;
        
        return spawnPosition;
    }

    private void Spawn(GameObject gameObject)
    {
        if(GameManager.Instance.GetCurGameState() == GameState.GameOver)
        {
            return;
        }

        GameObject enemyObject = Instantiate(gameObject, GetRandomPosition(), Quaternion.identity);    
        
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.SetTarget(_energyGenerator);

        GameManager.Instance.AddEnemy(enemy);
    }

    public Vector3 PlayerSpawnPos() => _playerStartPosition.position;
}
