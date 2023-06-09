using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private int powerUpID; // 0 = TripleShot, 1 = Shield, 2 = Ammo, 3 = Health, 4 = MegaShot, 5 = GoSlow, 6 = Missile
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _explosionAudioClip;

    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private SpawnManager _spawnManager;
    private Player _player;
    private bool _zoomToPlayer = false;
 
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Could not find AudioSource in PowerUp.");
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("Count not find SpriteRenderer in PowerUp.");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Could not find SpawnManager in PowerUp.");
        }
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_spawnManager == null)
        {
            Debug.LogError("Could not find Player in PowerUp.");
        }
    }

    void Update()
    {
        if (_zoomToPlayer && _player)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime * 2);
        }
        else
        {

            Vector3 powerUpMovement = new Vector3(0, -1, 0) * _speed * Time.deltaTime;

            transform.Translate(powerUpMovement);

            if (transform.position.y < -6.45)
            {
                _spawnManager.RemovePowerupFromBucket(this.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _spriteRenderer.enabled)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (powerUpID)
                {
                    case 0: // TripleShot
                        player.ActivateTripleShot();
                        break;
                    case 1: // Shield
                        player.ActivateShield();
                        break;
                    case 2: // Ammo
                        player.RefillAmmo();
                        break;
                    case 3: // Health
                        player.AddHealth();
                        break;
                    case 4: // Mega Laser
                        player.ActivateMegaLaser();
                        break;
                    case 5: // Go Slow
                        player.ActivateGoSlow();
                        break;
                    case 6: // Missile
                        player.ActivateMissile();
                        break;
                    default:
                        Debug.LogError("Unexpected PowerUpID value in PowerUp.");
                        break;
                }
            }
            _audioSource.Play();
            _spriteRenderer.enabled = false;
            StartCoroutine(DestroyCoolDown());
        }
        else if (other.tag == "EnemyLaser" && _spriteRenderer.enabled)
        {
            GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _audioSource.clip = _explosionAudioClip;
            _audioSource.Play();
            _spriteRenderer.enabled = false;
            Destroy(other.transform.gameObject);
            StartCoroutine(DestroyCoolDown());
        }
    }

    IEnumerator DestroyCoolDown()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    public void ZoomToPlayer()
    {
        _zoomToPlayer = true;
    }
}
