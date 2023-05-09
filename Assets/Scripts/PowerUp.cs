using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    [SerializeField]
    private int powerUpID; // 0 = TrippleShot, 1 = Speed, 2 = Shield
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;

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
    }

    void Update()
    {
        Vector3 powerUpMovement = new Vector3(0, -1, 0) * _speed * Time.deltaTime;

        transform.Translate(powerUpMovement);

        if (transform.position.y < -6.45)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _spriteRenderer.enabled)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch(powerUpID)
                {
                    case 0: // TrippleShot
                        player.TrippleShotActive();
                        break;
                    case 1: // Speed
                        player.SpeedActive();
                        break;
                    case 2: // Shield
                        player.ShieldActive();
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
    }

    IEnumerator DestroyCoolDown()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
