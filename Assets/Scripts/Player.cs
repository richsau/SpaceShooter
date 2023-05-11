using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _laserAudioClip;
    [SerializeField]
    private GameObject _rightWingFire;
    [SerializeField]
    private GameObject _leftWingFire;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject _laserTrippleShotPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _thrusterVisual;
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;
    private float _cooldownTime = 0.15f; 
    private float _canFireAgain = -1f;
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTrippleShotActive = false;
    private bool _isSpeedActive = false;
    private bool _isShieldActive = false;
    private int _score = 0;
    private UIManager _uiManager;
    private bool _leftWingOnFire = false;
    private AudioSource _audioSource;
    private int _speedFuel = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, -3.9f, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Could not find SpawnManager in Player.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Could not find AudioSource in Player.");
        } else
        {
            _audioSource.clip = _laserAudioClip;
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Could not find UIManager in Player.");
        }
        StartCoroutine(SpeedFuelFillUp());
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= _canFireAgain)
        {
            FireLaser();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSpeedActive)
        {
            ActivateSpeed();
        }
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 playerMovement = new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime;

        transform.Translate(playerMovement);
  
        // restrict x position
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.2f, 9.2f), transform.position.y, 0);

        // restrict y position
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.9f, 5.9f), 0);
    }

    void FireLaser()
    {
        _canFireAgain = Time.time + _cooldownTime;
        if (_isTrippleShotActive)
        {
            Instantiate(_laserTrippleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
        _audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            Destroy(other.gameObject);
            Damage();
        }
    }


    public void DamageToKill()
    {
        if (_isShieldActive)
        {
            _shieldVisual.SetActive(false);
            _isShieldActive = false;
        }
        else
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
            _uiManager.DisplayGameOver();
        }
    }
    
    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldVisual.SetActive(false);
            _isShieldActive = false;
            return; // exit out of damage method
        }
        
        _lives--;
        _uiManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 2:
                if (Random.Range(0, 2) == 0) // randomly pick left or right side first
                {
                    _leftWingOnFire = true;
                    _leftWingFire.SetActive(true);
                }
                else
                {
                    _rightWingFire.SetActive(true);
                }
                break;
            case 1:
                if (_leftWingOnFire)
                {
                    _rightWingFire.SetActive(true);
                }
                else
                {
                    _leftWingOnFire = true;
                    _leftWingFire.SetActive(true);
                }
                break;
            case 0:
                _spawnManager.StopSpawning();
                GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                _uiManager.DisplayGameOver();
                break;
            default:
                Debug.LogError("Unexpected lives count in Player.");
                break;
        }
    }

    public void TrippleShotActive()
    {
        if (!_isTrippleShotActive) // prevent more than one tripple shoot powerup at the same time
        {
            _isTrippleShotActive = true;
            StartCoroutine(TrippleShotCoolDown());
        }
    }

    IEnumerator TrippleShotCoolDown()
    {
        yield return new WaitForSeconds(5f);
        _isTrippleShotActive = false;
    }

    private void ActivateSpeed()
    {
        _isSpeedActive = true;
        _speed *= _speedMultiplier;
        _thrusterVisual.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // grow the thruster visual
        StartCoroutine(SpeedCoolDown());
    }

    IEnumerator SpeedCoolDown()
    {
        while (_speedFuel > 0)
        {
            yield return new WaitForSeconds(0.1f);
            _speedFuel--;
            _uiManager.UpdateSpeedFuel(_speedFuel);
        }
        _speed /= _speedMultiplier;
        _isSpeedActive = false;
        _thrusterVisual.transform.localScale = new Vector3(0.5f, 1.0f, 1.0f); // shrink the thruster visual
    }

    public void ShieldActive()
    {
        if (!_isShieldActive)  // prevent more than one shield powerup at the same time
        {
            _shieldVisual.SetActive(true);
            _isShieldActive = true;
        }
    }

    public void AddToScore(int amountToAdd)
    {
        _score += amountToAdd;
        _uiManager.UpdateScore(_score);
    }

    IEnumerator SpeedFuelFillUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(.25f);
            if (_speedFuel < 100 && !_isSpeedActive)
            {
                _speedFuel++;
                _uiManager.UpdateSpeedFuel(_speedFuel);
            }
        }
    }
}

