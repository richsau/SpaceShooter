using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _levelText;
    [SerializeField]
    private TMP_Text _ammoCountText;
    [SerializeField]
    private TMP_Text _missileReadyText;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _playerMessageText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Slider _speedFuel;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager could not be found in UIManager.");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager could not be found in UIManager.");
        }
        _scoreText.text = "Score: " + 0;
        HideGameOver();
        StartCoroutine(GameStartCoolDown());
    }

    public void UpdateLevel(int level)
    {
        StartCoroutine(NewLevelCoolDown(level));
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmo(int ammoCount, int maxAmmo)
    {
        _ammoCountText.text = "Ammo: " + ammoCount + " / " + maxAmmo;
    }

    public void BlinkAmmoText()
    {
        if (_ammoCountText.IsActive())
        {
            _ammoCountText.gameObject.SetActive(false);
        }
        else
        {
            _ammoCountText.gameObject.SetActive(true);
        }
    }

    public void BlinkMissileReadyText()
    {
        if (_missileReadyText.IsActive())
        {
            _missileReadyText.gameObject.SetActive(false);
        }
        else
        {
            _missileReadyText.gameObject.SetActive(true);
        }
    }

    public void DisableMissileReadyText()
    {
        _missileReadyText.gameObject.SetActive(false);
    }

    public void DisplayAmmoText()
    {
        _ammoCountText.gameObject.SetActive(true);
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _liveSprites[currentLives];
    }

    public void DisplayGameOver()
    {
        StartCoroutine(GameOverCoolDown());
    }

    public void HideGameOver()
    {
        _gameOverText.gameObject.SetActive(false);
    }

    public void UpdateSpeedFuel(int NewSpeedFuel)
    {
        _speedFuel.value = NewSpeedFuel;
    }

    IEnumerator GameOverCoolDown()
    {
        _gameOverText.fontSize = 0;
        _gameOverText.gameObject.SetActive(true);
        // adjust font size from 0 to 200
        for (int i = 0; i < 201; i++)
        {
            _gameOverText.fontSize = i;
            yield return new WaitForSeconds(.02f);
        }
        yield return new WaitForSeconds(3f);
        _gameOverText.gameObject.SetActive(false);
        _gameManager.SetGameOver(true);
        _playerMessageText.text = "Press 1 to play again or [ESC] to quit.";
        _playerMessageText.gameObject.SetActive(true);
    }

    IEnumerator GameStartCoolDown()
    {
        _playerMessageText.text = "Ready Player One";
        _playerMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _playerMessageText.gameObject.SetActive(false);
        _gameManager.SetGameOver(false);
        _spawnManager.StartSpawning();
    }

    IEnumerator NewLevelCoolDown(int level)
    {
        _playerMessageText.text = "Wave " + level;
        _playerMessageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _playerMessageText.gameObject.SetActive(false);
    }
}


