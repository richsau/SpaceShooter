using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserType2 : MonoBehaviour
{
    private float _speed = 8.0f;
    private Player _player;
    private Vector3 _fireDestination;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player could not be found in EnemyLaserType2.");
        }
        _fireDestination = _player.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(_fireDestination.x, _fireDestination.y, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        //transform.position = Vector3.MoveTowards(transform.position, _fireDestination, _speed * Time.deltaTime);
    }


        void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _fireDestination, _speed * Time.deltaTime);

        //transform.position += transform.forward * _speed * Time.deltaTime;

        //Vector3 laserMovement = new Vector3(0, 1, 0) * _speed * Time.deltaTime;

        //transform.Translate(laserMovement);


        //if (transform.position == _fireDestination)
        //{
        //    transform.position += transform.forward * _speed * Time.deltaTime;
        //}

        if (transform.position.y > 20.0)
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
}
