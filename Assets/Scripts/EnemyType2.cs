using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType2 : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyMinePrefab;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject _enemyLaserTargetPlayerPrefab;
    [SerializeField]
    private GameObject _enemyLaserTargetPowerupPrefab;
    private float _enemySpeed = 3.0f;
    private Player _player;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private bool _isVisible = false;
    private bool _isDestroyed = false;
    private int _currentXDirection = 0;
    private float _maxX = 9.7f;
    private float _minX = -9.7f;
    private Vector3 _fireDestination;
    private bool _shieldUsed = false;
    private GameObject _targetPowerup;



    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in Enemy.");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource could not be found in Enemy.");
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Could not find GameManager in Enemy.");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Could not find SpawnManager in Enemy.");
        }
        _isVisible = false;
        StartCoroutine(LayMine());
        StartCoroutine(ChangeDirections());
        StartCoroutine(ShieldManager());
        StartCoroutine(FireLaser());
    }

    // Update is called once per frame
    void Update()
    {
        if (_player)
        {
            if ((Vector3.Distance(transform.position, _player.transform.position) < 3.5f) && _shieldVisual.activeSelf)
            {
                transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _enemySpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(_currentXDirection, -1, 0) * _enemySpeed * Time.deltaTime);
            }
        } 
        else
        {
            transform.Translate(new Vector3(_currentXDirection, -1, 0) * _enemySpeed * Time.deltaTime);
        }
        
        if (transform.position.x > _maxX)
        {
            _currentXDirection = -1;
        }

        if (transform.position.x < _minX)
        {
            _currentXDirection = 1;
        }

        if (transform.position.y < -4.6 || transform.position.y > 7)
        {
            _isVisible = false;
        }
        else
        {
            _isVisible = true;
        }

        if (transform.position.y < -5.5)
        {
            float randomX = Random.Range(_minX, _maxX);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isVisible && !_gameManager.IsSuperPlayer())
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            DestroyEnemy();
        }

        if (((other.tag == "Laser") || (other.tag == "Missile")) && _isVisible)
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(10);
            }
            DestroyEnemy();
        }

        if (other.tag == "LaserRoute" && _isVisible)
        {
            if (transform.position.x < 0)
            {
                _currentXDirection = 1;
            }
            else
            {
                _currentXDirection = -1;
            }
            StartCoroutine(SpeedBurst());
        }
    }

    IEnumerator FireLaser()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 7));
            if (_isVisible && !_isDestroyed)
            {
                if ((_player.transform.position.y > transform.position.y) && (transform.position.y < -3))
                {
                    Instantiate(_enemyLaserTargetPlayerPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    _targetPowerup = _spawnManager.CheckPowerUpDistance(this.gameObject, 10f);
                    if (_targetPowerup)
                    {
                        Instantiate(_enemyLaserTargetPowerupPrefab, transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    IEnumerator LayMine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 7));
            if (_isVisible && !_isDestroyed)
            {
                Instantiate(_enemyMinePrefab, transform.position, Quaternion.identity);
            }
        }
    }

    // ChangeDirections
    // There's a 2 in 3 chance that the current direction 
    // will change every 1 to 3 seconds.
    IEnumerator ChangeDirections()
    {
        while (true)
        {
            _currentXDirection = Random.Range(-1, 2);
            yield return new WaitForSeconds(Random.Range(1, 4));
        }
    }

    IEnumerator ShieldManager()
    {
        while (true)
        {
            if(Random.Range(0, 100) < 10)
            {
                ActivateShield();
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpeedBurst()
    {
        _enemySpeed *= 2;
        yield return new WaitForSeconds(1);
        _enemySpeed /= 2;
    }


    private void ActivateShield()
    {
        if (!_shieldUsed)
        {
            _shieldUsed = true;
            _shieldVisual.SetActive(true);
        }
    }

    public void DestroyEnemy()
    {
        if (!_shieldVisual.activeSelf)
        {
            if (!_isDestroyed) // rare chance that object is destroyed while being destroyed
            {
                _spawnManager.RemoveEnemyFromBucket(this.gameObject);
                _isDestroyed = true;
                _spawnManager.UpdateEnimiesDestroyedCount();
                GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                _audioSource.Play();
                _enemySpeed = 0;
                Destroy(GetComponent<Collider2D>()); // prevent further collision while being destroyed
                Destroy(this.gameObject);
            }
        }
        else
        {
            GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _audioSource.Play();
            _shieldVisual.SetActive(false);
        }
    }
}
