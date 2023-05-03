using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;

    public void SetGameOver(bool isGameOver)
    {
        _isGameOver = isGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1); // load the game
        }
    }
}
