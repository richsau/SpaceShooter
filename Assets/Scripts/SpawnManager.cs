using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyType2Prefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _asteroidPrefab;
    [SerializeField]
    private GameObject _bossPrefab;
    [SerializeField]
    private GameObject[] _powerupPrefabs;
    List<GameObject> _powerupBucket = new List<GameObject>();
    List<GameObject> _enemyBucket = new List<GameObject>();
    private GameManager _gameManager;
    private GameObject _boss;
    private int _powerUpRoll;
    private int _powerUpType;
    private bool _okToSpawn = false;
    private int _enemiesSpawned = 0;
    private int _enemiesDestroyed = 0;
    private bool _bossSpawned = false;

    private enum _powerUpTypes
    {
        Triple,
        Shield,
        Ammo,
        Health,
        Mega,
        GoSlow,
        Missile
    }

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Could not find GameManager in SpawnManager.");
        }
    }

    public void StartSpawning()
    {
        _okToSpawn = true;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpwanPowerUp());
        StartCoroutine(SpawnAsteroid());
    }

    public void StopSpawning()
    {
        _okToSpawn = false;
    }

    private void SpawnNewEnemy()
    {
        float randomX = Random.Range(-8.5f, 8.5f);
        Vector3 enemySpawnLocation = new Vector3(randomX, 7.5f, 0);
        GameObject newEnemy = Instantiate(_enemyPrefab, enemySpawnLocation, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemyBucket.Add(newEnemy);
        _enemiesSpawned++;
    }

    private void SpawnNewEnemyType2()
    {
        float randomX = Random.Range(-8.5f, 8.5f);
        Vector3 enemySpawnLocation = new Vector3(randomX, 7.5f, 0);
        GameObject newEnemy = Instantiate(_enemyType2Prefab, enemySpawnLocation, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemyBucket.Add(newEnemy);
        _enemiesSpawned++;
    }

    private void SpawnBoss()
    {
        float randomX = Random.Range(-8.5f, 8.5f);
        Vector3 enemySpawnLocation = new Vector3(randomX, 7.5f, 0);
        GameObject newEnemy = Instantiate(_bossPrefab, enemySpawnLocation, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
        _boss = newEnemy;
    }


    public GameObject FindMissileTarget(GameObject missile)
    {
        float currentDistance = 1000f;
        float newDistance;
        GameObject currentTarget = null;

        foreach (GameObject enemy in _enemyBucket)
        {
            if (missile && enemy)
            {
                newDistance = Vector3.Distance(missile.transform.position, enemy.transform.position);
                if (newDistance < currentDistance)
                {
                    currentTarget = enemy;
                    currentDistance = newDistance;
                }
            }
        }
        return currentTarget;
    }

    public void UpdateEnimiesDestroyedCount()
    {
        _enemiesDestroyed++;
    }

    public void RemoveEnemyFromBucket(GameObject enemy)
    {
        _enemyBucket.Remove(enemy);
    }


    public void RemovePowerupFromBucket(GameObject powerup)
    {
        _powerupBucket.Remove(powerup);
    }

    public GameObject CheckPowerUpDistance(GameObject target, float distance)
    {
        foreach (GameObject powerUp in _powerupBucket)
        {
            if (target && powerUp)
            {
                if (Vector3.Distance(powerUp.transform.position, target.transform.position) < distance)
                {
                    return powerUp;
                }
            }
        }
        return null;
    }
    IEnumerator SpawnEnemy()
    {
        while (_okToSpawn == true)
        {
            int level;

            level = _gameManager.GetLevel();

            if ((level % 5 == 0) && (level > 0))
            {
                if (!_bossSpawned)
                {
                    SpawnBoss();
                    _bossSpawned = true;
                }
                else
                {
                    if (!_boss) // boss has been destroyed
                    {
                        _bossSpawned = false;
                        _okToSpawn = false;
                        _bossSpawned = false;
                        _gameManager.NewLevel();
                        _enemiesSpawned = 0;
                        _enemiesDestroyed = 0;
                        _okToSpawn = true;
                    }
                }
            }
            else if (_enemiesSpawned < (level * 5) && (level > 0))
            {
                if (Random.Range(0, 100) < 5)
                {
                    SpawnNewEnemyType2();
                }
                else
                {
                    SpawnNewEnemy();
                }
            }
            else
            {
                if (_enemiesDestroyed == _enemiesSpawned)
                {
                    _okToSpawn = false;
                    _bossSpawned = false;
                    _gameManager.NewLevel();
                    _enemiesSpawned = 0;
                    _enemiesDestroyed = 0;
                    _okToSpawn = true;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpwanPowerUp()
    {
        while (_okToSpawn == true)
        {
            _powerUpRoll = Random.Range(0, 100);
            float randomX = Random.Range(-8.5f, 8.5f);
            Vector3 powerUpSpawnLocation = new Vector3(randomX, 7.5f, 0);
            if (_powerUpRoll < 20) // 20%
            {
                _powerUpType = (int)_powerUpTypes.Triple;
            }
            else if (_powerUpRoll > 19 && _powerUpRoll < 50) // 30%
            {
                _powerUpType = (int)_powerUpTypes.Shield;
            }
            else if (_powerUpRoll > 49 && _powerUpRoll < 80) // 30%
            {
                _powerUpType = (int)_powerUpTypes.Ammo;
            }
            else if (_powerUpRoll > 79 && _powerUpRoll < 85) // 5%
            {
                _powerUpType = (int)_powerUpTypes.Health;
            }
            else if (_powerUpRoll > 84 && _powerUpRoll < 90) // 5%
            {
                _powerUpType = (int)_powerUpTypes.Missile;
            }
            else if (_powerUpRoll > 89 && _powerUpRoll < 95) // 5%
            {
                _powerUpType = (int)_powerUpTypes.Mega;
            }
            else if (_powerUpRoll > 94) // 5%
            {
                _powerUpType = (int)_powerUpTypes.GoSlow;
            }
            else
            {
                Debug.LogError("Unexpected _powerUpRoll in SpawnManager: " + _powerUpRoll);
            }

            if (_gameManager.GetLevel() > 0)
            {
                GameObject NewPowerup = Instantiate(_powerupPrefabs[_powerUpType], powerUpSpawnLocation, Quaternion.identity);
                _powerupBucket.Add(NewPowerup);
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


}

