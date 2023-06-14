using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _laserAudioClip;
    [SerializeField]
    private AudioClip _missileAudioClip;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    [SerializeField]
    private GameObject _rightWingFire;
    [SerializeField]
    private GameObject _leftWingFire;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject _laserTripleShotPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _thrusterVisual;
    [SerializeField]
    private GameObject _megaLaserPrefab;
    [SerializeField]
    private GameObject _missilePrefab;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private CameraShake _cameraShake;
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;
    private float _cooldownTime = 0.15f;
    private float _canFireAgain = -1f;
    private int _lives = 3;
    private bool _isTripleShotActive = false;
    private bool _isMegaLaserActive = false;
    private bool _isSpeedActive = false;
    private bool _isSlowActitve = false;
    private int _shieldLevel = 0;
    private int _score = 0;
    private bool _leftWingOnFire = false;
    private int _speedFuel = 0;
    private int _ammoCount = 15;
    private int _maxAmmo = 20;
    private bool _playerIsLucky;
    private bool _missileAvailable = false;

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
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Could not find UIManager in Player.");
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Could not find GameManager in Player.");
        }
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("Could not find CameraShake in Player.");
        }
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
        StartCoroutine(SpeedFuelFillUp());
        StartCoroutine(LowAmmoCheck());
        StartCoroutine(PlayerLuckCheck());
    }

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
        if (Input.GetKeyDown(KeyCode.C))
        {
            ActivatePowerupZoom();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_missileAvailable)
            {
                FireMissile();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser" && !_gameManager.IsSuperPlayer())
        {
            Destroy(other.gameObject);
            _audioSource.clip = _explosionAudioClip;
            _audioSource.Play();
            Damage();
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
        _audioSource.clip = _laserAudioClip;
        _canFireAgain = Time.time + _cooldownTime;
        if (_isMegaLaserActive)
        {
            Instantiate(_megaLaserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _audioSource.Play();
        }
        else
        {
            if (_ammoCount > 0)
            {
                if (_isTripleShotActive)
                {
                    Instantiate(_laserTripleShotPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                }
                _ammoCount--;
                _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
                _audioSource.Play();
            }
        }
    }

    void FireMissile()
    {
        _audioSource.clip = _missileAudioClip;
        _missileAvailable = false;
        _uiManager.DisableMissileReadyText();
        Instantiate(_missilePrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        _audioSource.Play();
    }



    public void DamageToKill()
    {
        _cameraShake.StartCameraShake();
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
        _cameraShake.StartCameraShake();
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
                }
                else
                {
                    _rightWingFire.SetActive(false);
                }
                break;
            case 2: // one wing on fire
                if (_leftWingOnFire)
                {
                    _leftWingFire.SetActive(false);
                }
                else
                {
                    _rightWingFire.SetActive(false);
                }
                break;
            default:
                Debug.Log("No impact in lives count in Player.AddHealth");
                break;
        }
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }

    }

    public void RefillAmmo()
    {
        _ammoCount += 15;
        if (_ammoCount > _maxAmmo)
        {
            _ammoCount = _maxAmmo;
        }
        _uiManager.UpdateAmmo(_ammoCount, _maxAmmo);
    }

    public void ActivateTripleShot()
    {
        if (!_isTripleShotActive) // prevent more than one at a time
        {
            _isTripleShotActive = true;
            StartCoroutine(TripleShotCoolDown());
        }
    }

    public void ActivateMegaLaser()
    {
        if (!_isMegaLaserActive) // prevent more than one at a time
        {
            _isMegaLaserActive = true;
            StartCoroutine(MegaLaserCoolDown());
        }
    }

    private void ActivatePowerupZoom()
    {
        PowerUp targetPowerup;

        GameObject target = _spawnManager.CheckPowerUpDistance(this.gameObject, 10f);
        if (target)
        {
            targetPowerup = GameObject.FindObjectOfType<PowerUp>(target);
            targetPowerup.ZoomToPlayer();
        }
    }

    private void ActivateSpeed()
    {
        if (!_isSlowActitve)
        {
            _isSpeedActive = true;
            _speed *= _speedMultiplier;
            _thrusterVisual.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // grow the thruster visual
            StartCoroutine(SpeedCoolDown());
        }
    }

    public void ActivateMissile()
    {
        _missileAvailable = true;
        StartCoroutine(MissileReady());
    }

    public void ActivateGoSlow()
    {
        if (!_isSpeedActive)
        {
            _isSlowActitve = true;
            _speed /= _speedMultiplier;
            _thrusterVisual.transform.localScale = new Vector3(0.25f, 1.0f, 1.0f); // shrink the thruster visual
            StartCoroutine(SlowCoolDown());
        }

    }
    public void ActivateShield()
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

    public bool IsPlayerLucky()
    {
        return _playerIsLucky;
    }
    IEnumerator MissileReady()
    {
        while (_missileAvailable)
        {
            _uiManager.BlinkMissileReadyText();
            yield return new WaitForSeconds(0.5f);
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

    IEnumerator TripleShotCoolDown()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    IEnumerator MegaLaserCoolDown()
    {
        yield return new WaitForSeconds(5f);
        _isMegaLaserActive = false;
    }

    IEnumerator PlayerLuckCheck()
    {
        while (true)
        {
            if (Random.Range(0, 100) < 5)
            {
                _playerIsLucky = true;
            }
            else
            {
                _playerIsLucky = false;
            }
            yield return new WaitForSeconds(10f);
        }
    }

    private IEnumerator SlowCoolDown()
    {
        yield return new WaitForSeconds(5f);
        _speed *= _speedMultiplier;
        _isSlowActitve = false;
        _thrusterVisual.transform.localScale = new Vector3(0.5f, 1.0f, 1.0f); // grow the thruster visual
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

