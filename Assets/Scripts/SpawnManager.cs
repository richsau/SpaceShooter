using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerupPrefabs;

    private int _powerUpType;
    private bool _stopSpawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpwanPowerUp());
    }

    IEnumerator SpawnEnemy()
    {
        while( _stopSpawning == false )
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 enemySpawnLocation = new Vector3(randomX, 7.5f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemySpawnLocation, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpwanPowerUp()
    {
        while (_stopSpawning == false)
        {
            _powerUpType = Random.Range(0, 2);
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 powerUpSpawnLocation = new Vector3(randomX, 7.5f, 0);
            Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5, 11));
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}

