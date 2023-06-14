using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    private float _speed = 8.0f;

    void Update()
    {
        Vector3 laserMovement = new Vector3(0, -1, 0) * _speed * Time.deltaTime;

        transform.Translate(laserMovement);

        if (transform.position.y < -5.8)
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

    public void Destroy()
    {
        Destroy(this);
    }
}
