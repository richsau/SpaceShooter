using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _cooldownTime = 0.15f; 
    private float _canFireAgain = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        //make the current position = new position(0, -3.9, 0)
        transform.position = new Vector3(0, -3.9f, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is null.");
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
        Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
    }

    public void Damage()
    {
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }

    }

}

