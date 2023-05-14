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
    private GameObject _asteroidPrefab;
    [SerializeField]
    private GameObject[] _powerupPrefabs;
    private int _powerUpType;
    private bool _okToSpawn = false;
    private bool _OkToSoawnHealth = false;
    
    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        while (_okToSpawn == true)
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
        yield return new WaitForSeconds(Random.Range(5, 11));
        while (_okToSpawn == true)
        {
            _powerUpType = Random.Range(0, 4);
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 powerUpSpawnLocation = new Vector3(randomX, 7.5f, 0);
            switch(_powerUpType)
            {
                case 3:
                    if (_OkToSoawnHealth)
                    {
                        Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    }
                    break;
                default:
                    Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    break;
            }
            yield return new WaitForSeconds(Random.Range(1, 2));
            //yield return new WaitForSeconds(Random.Range(5, 11));
        }
    }

    IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(Random.Range(7, 15));
        while (_okToSpawn == true)
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 asteroidSpawnLocation = new Vector3(randomX, 7.5f, 0);
            Instantiate(_asteroidPrefab, asteroidSpawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(7, 15));
        }
    }

    public void StopSpawning()
    {
        _okToSpawn = false;
    }

    public void SetHealthSpawn(bool OkToSpawn)
    {
        _OkToSoawnHealth = OkToSpawn;
    }

    public void StartSpawning()
    {
        _okToSpawn = true;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpwanPowerUp());
        StartCoroutine(SpawnAsteroid());
    }
}

