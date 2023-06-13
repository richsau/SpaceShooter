using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserTypePlayer : MonoBehaviour
{
    private float _speed = 8.0f;
    private Player _player;
 
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in EnemyLaserTypePlayer.");
        }
        else
        {
            FaceTarget();
        }
    }

    void Update()
    {
        if (_player)
        {
            transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);
            if ((transform.position.y < -5.8) || (transform.position.y > 7.2) ||
                (transform.position.x < -10.4) || (transform.position.x > 10.4))
            {
                if (transform.parent == null)
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void FaceTarget()
    {
        Vector3 offset = _player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), offset);
        transform.rotation = rotation;
    }

}
