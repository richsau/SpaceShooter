using UnityEngine;

public class Missile : MonoBehaviour
{
    private GameObject _target = null;
    private SpawnManager _spawnManager;
    private float _speed = 8.0f;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager could not be found in Missile.");
        }
        else
        {
            _target = _spawnManager.FindMissileTarget(this.gameObject);
            if (_target)
            {
                FaceTarget();
            }
        }
    }

    void Update()
    {
        if (_target)
        {
            FaceTarget();
        }
        else
        {
            _target = _spawnManager.FindMissileTarget(this.gameObject);
        }

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

    private void FaceTarget()
    {
        Vector3 offset = _target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), offset);
        transform.rotation = rotation;
    }
}
