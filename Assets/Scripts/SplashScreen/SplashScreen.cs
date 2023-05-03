using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{

    public void LoadGame()
    {
        SceneManager.LoadScene(1); // load game scene
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SplashScreenCoolDown());
    }

    IEnumerator SplashScreenCoolDown()
    {
        yield return new WaitForSeconds(3f);
        LoadGame();
    }
}
