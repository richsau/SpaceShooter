using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private Transform[] path;
    [SerializeField]
    private Transform[] _destroyPoints;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _bigExplosionPrefab;
    public float speed = 0.5f;
    public float reachDistance = 1.0f;
    //public int currentPoint = 0;
    private int _movIndex = 0;
    private int _numGuns = 10;
    private bool _isDestroyed = false;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        path[0] = GameObject.FindWithTag("Mov0").transform;
        path[1] = GameObject.FindWithTag("Mov1").transform;
        path[2] = GameObject.FindWithTag("Mov2").transform;
        path[3] = GameObject.FindWithTag("Mov3").transform;
        path[4] = GameObject.FindWithTag("Mov4").transform;
        path[5] = GameObject.FindWithTag("Mov5").transform;
        path[6] = GameObject.FindWithTag("Mov6").transform;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource could not be found in Boss.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(path[_movIndex].position, transform.position);
        transform.position = Vector3.MoveTowards(transform.position, path[_movIndex].position, Time.deltaTime * speed);

        if (distance <= reachDistance)
        {
            _movIndex = Random.Range(0, 7);
        }
    }

    //private void DestroyBoss()
    //{
    //    _isDestroyed = true;
    //    speed = 0;
    //    foreach (Transform T in _destroyPoints)
    //    {
    //        Instantiate(_explosionPrefab, T.position, Quaternion.identity);
    //        _audioSource.Play();
    //    }
    //    Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
    //    Destroy(this.gameObject);

    //}


    public void GunDestroyed()
    {
        _numGuns--;
        if (_numGuns < 1)
        {
            if (!_isDestroyed)
            {
                StartCoroutine(DestroyBoss());
            }
        }
    }

    IEnumerator DestroyBoss()
    {
        _isDestroyed = true;
        speed = 0;
        foreach (Transform T in _destroyPoints)
        {
            Instantiate(_explosionPrefab, T.position, Quaternion.identity);
            _audioSource.Play();
            yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        }
        Instantiate(_bigExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

}
