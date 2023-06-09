using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyLaserPrefab;
    private float _enemySpeed = 2.0f;
    private Player _player;
    private Animator _enemyDeathAnim;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private bool _isVisible = false;
    private bool _isDestroyed = false;
    private int _currentXDirection = 0;
    private float _maxX = 9.7f;
    private float _minX = -9.7f;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in Enemy.");
        }
        _enemyDeathAnim = GetComponent<Animator>();
        if (_enemyDeathAnim == null)
        {
            Debug.LogError("Animator could not be found in Enemy.");
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
        StartCoroutine(FireLaser());
        StartCoroutine(ChangeDirections());
    }

    void Update()
    {
        transform.Translate(new Vector3(_currentXDirection, -1, 0) * _enemySpeed * Time.deltaTime);
        if (transform.position.x > _maxX)
        {
            _currentXDirection = -1;
        }

        if (transform.position.x < _minX)
        {
            _currentXDirection = 1;
        }

        if (transform.position.y < 7.44)
        {
            _isVisible = true;
        }

        if (transform.position.y < -5.5)
        {
            _isVisible = false;
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
    }

    public void DestroyEnemy()
    {
        if (!_isDestroyed) // rare chance that object is destroyed while being destroyed
        {
            _spawnManager.RemoveEnemyFromBucket(this.gameObject);
            _isDestroyed = true;
            _spawnManager.UpdateEnimiesDestroyedCount();
            _enemyDeathAnim.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            _enemySpeed = 0;
            Destroy(GetComponent<Collider2D>()); // prevent further collision while being destroyed
            Destroy(this.gameObject, 2.8f);
        }
    }
    IEnumerator FireLaser()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, 7));
            if (_isVisible && !_isDestroyed)
            {
                Instantiate(_enemyLaserPrefab, transform.position, Quaternion.identity);
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
}

