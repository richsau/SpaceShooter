using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;
    private Player _player;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private int _rotateDirection;
    private bool _isVisible = false;
    private float _rotateSpeed = 20.0f;
    private float _speed = 10.0f;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in Asteroid.");
        }

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager could not be found in Asteroid.");
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager could not be found in Asteroid.");
        }
        _rotateDirection = Random.Range(-1, 2);
        _isVisible = false;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotateDirection) * _rotateSpeed * Time.deltaTime);

        Vector3 asteroidMovement = new Vector3(0, -1, 0) * _speed * Time.deltaTime;

        transform.Translate(asteroidMovement);

        if (transform.position.y < 7.1)
        {
            _isVisible = true;
        }

        if (transform.position.y < -6.45)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isVisible && !_gameManager.IsSuperPlayer())
        {
            if (_player != null)
            {
                _player.DamageToKill();
            }
            DestroyAsteroid();
        }

        if (other.tag == "Laser" && _isVisible)
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(100);
            }
            DestroyAsteroid();
        }

        if (other.tag == "Enemy" && _isVisible)
        {
            Enemy _enemy = other.GetComponent<Enemy>();
            if (_enemy)
            {
                _enemy.DestroyEnemy();
            }
            DestroyAsteroid();
        }
    }

    private void DestroyAsteroid()
    {
        GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
