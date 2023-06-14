using System.Collections;
using UnityEngine;

public class BossGun : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _enemyLaserTargetPlayerPrefab;
    private Player _player;
    private Boss _boss;
    private AudioSource _audioSource;
    private bool _isDestroyed = false;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in BossGun.");
        }
        _boss = transform.parent.GetComponent<Boss>();
        if (_boss == null)
        {
            Debug.LogError("Boss could not be found in BossGun.");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource could not be found in BossGun.");
        }
        StartCoroutine(FireLaser());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Laser") || (other.tag == "Missile"))
        {
            Destroy(other.gameObject);
            if (_player)
            {
                _player.AddToScore(100);
            }
            DestroyGun();
        }
    }

    public void DestroyGun()
    {
        if (!_isDestroyed) // rare chance that object is destroyed while being destroyed
        {
            _isDestroyed = true;
            GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _audioSource.Play();
            _boss.GunDestroyed();
            Destroy(GetComponent<Collider2D>()); // prevent further collision while being destroyed
            Destroy(this.gameObject);
        }
    }

    IEnumerator FireLaser()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 15));
            if (!_isDestroyed)
            {
                Instantiate(_enemyLaserTargetPlayerPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
