using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        //make the current position = new position(0, -3.9, 0)
        transform.position = new Vector3(0, -3.9f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
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

}


//// restrict x position
//transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9.2f, 9.2f), transform.position.y, 0);

//// restrict y position
//transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.9f, 5.9f), 0);
