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
    private bool _okToSpawnHealth = false;
    private bool _okToSpawnMegaLaser = false;
    private bool _okToSpawnAmmo = true;
    private GameManager _gameManager;
    private int _enemiesSpawned = 0;


    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Could not find GameManager in SpawnManager.");
        }
    }


    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        
        while (_okToSpawn == true)
        {
            int level;

            level = _gameManager.GetLevel();

            if (_enemiesSpawned < (level * 5) && (level > 0))
            {
                SpawnNewEnemy();
            } else
            {
                _okToSpawn = false;
                _gameManager.NewLevel();
                _enemiesSpawned = 0;
                _okToSpawn = true;
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void SpawnNewEnemy()
    {
        float randomX = Random.Range(-8.5f, 8.5f);
        Vector3 enemySpawnLocation = new Vector3(randomX, 7.5f, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, enemySpawnLocation, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemiesSpawned++;  
    }

    IEnumerator SpwanPowerUp()
    {
        while (_okToSpawn == true && _gameManager.GetLevel() > 0)
        {
            _powerUpType = Random.Range(0, 5);
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 powerUpSpawnLocation = new Vector3(randomX, 7.5f, 0);
            switch(_powerUpType)
            {
                case 2:
                    if (_okToSpawnAmmo)
                    {
                        Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    }
                    break;
                case 3:
                    if (_okToSpawnHealth)
                    {
                        Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    }
                    break;
                case 4:
                    if (_okToSpawnMegaLaser)
                    {
                        Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    }
                    break;
                default:
                    Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                    break;
            }
            yield return new WaitForSeconds(Random.Range(5, 11));
        }
    }

    IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(Random.Range(20, 60));
        while (_okToSpawn == true && _gameManager.GetLevel() > 0)
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 asteroidSpawnLocation = new Vector3(randomX, 7.5f, 0);
            Instantiate(_asteroidPrefab, asteroidSpawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(20, 60));
        }
    }

    public void StopSpawning()
    {
        _okToSpawn = false;
    }

    public void SetHealthSpawn(bool OkToSpawn)
    {
        _okToSpawnHealth = OkToSpawn;
    }

    public void SetAmmoSpawn(bool OkToSpawn)
    {
        _okToSpawnAmmo = OkToSpawn;
    }

    public void SetMegaLaserSpawn(bool OkToSpawn)
    {
        _okToSpawnMegaLaser = OkToSpawn;
    }

    public void StartSpawning()
    {
        _okToSpawn = true;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpwanPowerUp());
        StartCoroutine(SpawnAsteroid());
    }
}

