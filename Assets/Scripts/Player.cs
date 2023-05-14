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
    private int _shieldLevel = 0;
    private int _score = 0;
    private UIManager _uiManager;
    private bool _leftWingOnFire = false;
    private AudioSource _audioSource;
    private int _speedFuel = 0;
    private int _ammoCount = 15;
    
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
        _spawnManager.SetHealthSpawn(false);
        StartCoroutine(SpeedFuelFillUp());
        StartCoroutine(LowAmmoCheck());
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
        if (_ammoCount > 0)
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
            _ammoCount--;
            _uiManager.UpdateAmmo(_ammoCount);
        }
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
        if (_shieldLevel > 0)
        {
            _shieldVisual.SetActive(false);
            _shieldLevel = 0;
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
        if (_shieldLevel > 0)
        {
            switch (_shieldLevel)
            {
                case 3:
                    _shieldLevel = 2;
                    _shieldVisual.transform.localScale = new Vector3(1, 2, 1);
                    break;
                case 2:
                    _shieldLevel = 1;
                    _shieldVisual.transform.localScale = new Vector3(.7f, 2, 1);
                    break;
                case 1:
                    _shieldLevel = 0;
                    _shieldVisual.SetActive(false);
                    break;
                default:
                    Debug.LogError("Unexpected shield level in Player.");
                    break;
            }
            return; // exit out of damage method
        }
        
        _lives--;
        _uiManager.UpdateLives(_lives);
        _spawnManager.SetHealthSpawn(true);

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
                Debug.LogError("Unexpected lives count in Player.Dammage");
                break;
        }
    }

    public void AddHealth()
    {
        switch (_lives)
        {
            case 1: // both wings are on fire
                if (Random.Range(0, 2) == 0) // randomly pick one to repair
                {
                    _leftWingOnFire = false;
                    _leftWingFire.SetActive(false);
                } else
                {
                    _rightWingFire.SetActive(false);
                }
                _spawnManager.SetHealthSpawn(true);
                break;
            case 2: // one wing on fire
                if (_leftWingOnFire)
                {
                    _leftWingFire.SetActive(false);
                } else
                {
                    _rightWingFire.SetActive(false);
                }
                _spawnManager.SetHealthSpawn(false);
                break;
            default:
                Debug.LogError("Unexpected lives count in Player.AddHealth");
                break;
        }
        _lives++;
        _uiManager.UpdateLives(_lives);
    }


    public void RefillAmmo()
    {
        _ammoCount += 15;
        _uiManager.UpdateAmmo(_ammoCount);
    }

    public void TrippleShotActive()
    {
        if (!_isTrippleShotActive) // prevent more than one tripple shoot powerup at the same time
        {
            _isTrippleShotActive = true;
            StartCoroutine(TrippleShotCoolDown());
        }
    }

    IEnumerator LowAmmoCheck()
    {
        while (true)
        {
            if (_ammoCount < 6)
            {
                _uiManager.BlinkAmmoText();
            }
            else
            {
                _uiManager.DisplayAmmoText();
            }
            yield return new WaitForSeconds(0.5f);
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
         _shieldVisual.SetActive(true);
         _shieldLevel = 3;
         _shieldVisual.transform.localScale = new Vector3(2, 2, 1);
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

