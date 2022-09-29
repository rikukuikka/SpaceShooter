using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    private GameManager _gameManager;
    private int _bestScore;
    [SerializeField]
    private Text _bestScoreText;
    private int _score;

    // Start is called before the first frame update
    void Start()
    {
        _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        _scoreText.text = "Score: 0";
        _bestScoreText.text = "Best: " + _bestScore;
        _restartText.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        _score = score;
        _scoreText.text = "Score: " + _score;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            currentLives = 0;
        }

        _livesImg.sprite = _liveSprites[currentLives];
        if (currentLives == 0)
        {
            UpdateBestScore();
            GameOverSequence();
        }
    }

    private void UpdateBestScore()
    {
        if (_score > _bestScore)
        {
            _bestScore = _score;
            _bestScoreText.text = "Best: " + _bestScore;
            PlayerPrefs.SetInt("BestScore", _bestScore);
        }
    }

    private void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(FlickerGameOverRoutine());
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
    }


    IEnumerator FlickerGameOverRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            bool gameOverActive = !_gameOverText.IsActive();
            _gameOverText.gameObject.SetActive(gameOverActive);
        }
    }
}
