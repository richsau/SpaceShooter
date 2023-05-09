using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
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
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Exiting game...");
    }
}
