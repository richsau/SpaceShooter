using UnityEngine;

public class EnemyLaserTypePowerup : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private GameObject _targetPowerup = null;
    private float _speed = 8.0f;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Could not find SpawnManager in EnemyLaserTypePowerup.");
        }
        else
        {
            _targetPowerup = _spawnManager.CheckPowerUpDistance(this.gameObject, 10f);
            if (!_targetPowerup)
            {
                DestroyLaser();
            }
        }
    }

    void Update()
    {
        if (_targetPowerup)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPowerup.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);
        }

        if ((transform.position.y < -5.8) || (transform.position.y > 7.2) ||
            (transform.position.x < -10.4) || (transform.position.x > 10.4))
        {
            DestroyLaser();
        }
    }

    private void DestroyLaser()
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

    private void FaceTarget()
    {
        Vector3 offset = _targetPowerup.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), offset);
        transform.rotation = rotation;
    }
}
