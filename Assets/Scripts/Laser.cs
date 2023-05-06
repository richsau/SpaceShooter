using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//todo: make sure item is visible to destroy anything

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 laserMovement = new Vector3(0, 1, 0) * _speed * Time.deltaTime;

        transform.Translate(laserMovement);

        if (transform.position.y > 8.0)
        {
            if (transform.parent == null)
            {
                Destroy(this.gameObject);
            } else
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}

