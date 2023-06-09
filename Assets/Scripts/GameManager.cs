using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;
    private bool _isSuperPlayer = false;
    private bool _isGameOver = false;
    private int _level = 0;

    public void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Could not find UIManager in GameManager.");
        }
    }

    void Update()
    {
        int goTOLevel = 0;

        if (_isGameOver && Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1); // load the game
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            goTOLevel = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            goTOLevel = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            goTOLevel = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            goTOLevel = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            goTOLevel = 6;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (_isSuperPlayer)
            {
                Debug.Log("Turning OFF SuperPlayer");
                _isSuperPlayer = false;
            }
            else
            {
                Debug.Log("Turning ON SuperPlayer");
                _isSuperPlayer = true;
            }
        }

        if (goTOLevel != 0)
        {
            Debug.Log("Switching to level: " + goTOLevel);
            SetLevel(goTOLevel);
        }
#endif
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void SetGameOver(bool isGameOver)
    {
        _isGameOver = isGameOver;
    }


    private void OnApplicationQuit()
    {
        Debug.Log("Exiting game...");
    }

    public int GetLevel()
    {
        return _level;
    }

    public void NewLevel()
    {
        _level++;
        _uiManager.UpdateLevel(_level);
    }

    public bool IsSuperPlayer()
    {
        return _isSuperPlayer;
    }

    private void SetLevel(int level)
    {
        _level = level;
        _uiManager.UpdateLevel(_level);
    }
}
