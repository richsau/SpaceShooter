using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _laserTrippleShotPrefab;
    [SerializeField]
    private float _cooldownTime = 0.15f; 
    private float _canFireAgain = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTrippleShotActive = false;
    [SerializeField]
    private bool _isSpeedActive = false;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private int _score = 0;
    private UIManager _uiManager;
    private bool _leftWingOnFire = false;
    //private bool _rightWingOnFire = false;
    [SerializeField]
    private GameObject _leftWingFire;
    [SerializeField]
    private GameObject _rightWingFire;
    [SerializeField]
    private AudioClip _laserAudioClip;
    private AudioSource _audioSource;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, -3.9f, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is null.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource could not be found in Player.");
        } else
        {
            _audioSource.clip = _laserAudioClip;
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= _canFireAgain)
        {
            FireLaser();
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

    public void SpeedActive()
    {
        if (!_isSpeedActive) // prevent more than one speed powerup at the same time
        {
            _isSpeedActive = true;
            _speed *= _speedMultiplier;
            StartCoroutine(SpeedCoolDown());
        }
    }

    IEnumerator SpeedCoolDown()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedMultiplier;
        _isSpeedActive = false;
    }

    public void ShieldActive()
    {
        if (!_isShieldActive)
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
}

