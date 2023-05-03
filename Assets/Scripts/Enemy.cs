using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _enemySpeed = 4.0f;
    private Player _player;
    private Animator _enemyDeathAnim;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player can't be found in Enemy.");
        }
        _enemyDeathAnim = GetComponent<Animator>();
        if (_enemyDeathAnim == null)
        {
            Debug.LogError("Animator can't be found in Enemy.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 enemyMovement = new Vector3(0, -1, 0) * _enemySpeed * Time.deltaTime;

        transform.Translate(enemyMovement);

        if (transform.position.y < -5.5)
        {
            float randomX = Random.Range(-8.5f, 8.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            DestroyEnemy();
        }

        if (other.tag == "Laser")
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
        _enemyDeathAnim.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0; 
        Destroy(this.gameObject, 2.8f);
    }
}

