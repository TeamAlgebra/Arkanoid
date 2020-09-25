using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text hightScoreText;
    public Text lifesText;

    private int score;
    private int hightScore;
    private int lives;

    private static UIManager instance;

    public static UIManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<UIManager>();
            if (instance == null)
            {
                GameObject container = new GameObject("UIManager");
                instance = container.AddComponent<UIManager>();
            }
        }

        return instance;
    }

    private void Start()
    {
        score = 0;
        lives = 3;

        hightScore = PlayerPrefs.GetInt("hight_score", 0);
        UpdateUI();

        GameManager gameManager = GameManager.GetInstance();

        gameManager.OnRestartGame += onRestartGame;
        gameManager.OnScorePoints += onRestartGame;
        gameManager.OnLossLife += onLossLife;
    }

    private void onRestartGame(int score)
    {
        this.score += score;
        UpdateUI();
    }

    private void onLossLife()
    {
        lives--;
        UpdateUI();
        if (lives <= 0)
        {
            GameManager gameManager = GameManager.GetInstance();
            if (score > hightScore)
                PlayerPrefs.SetInt("hight_score", score);
            score = 0;
            gameManager.RestartGame();
        }
    }

    private void onRestartGame()
    {
        if (score > hightScore)
            PlayerPrefs.SetInt("hight_score", score);
        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"{score}";
        hightScoreText.text = $"{hightScore}";
        lifesText.text = $"Lives: {lives}";
    }

    private void OnApplicationQuit()
    {
        if (score > hightScore)
            PlayerPrefs.SetInt("hight_score", score);

        if (instance == this)
            instance = null;
    }

    private void OnDestroy()
    {
        if (score > hightScore)
            PlayerPrefs.SetInt("hight_score", score);

        GameManager gameManager = GameManager.GetInstance();
        gameManager.OnRestartGame -= onRestartGame;
        gameManager.OnScorePoints -= onRestartGame;
        gameManager.OnLossLife -= onLossLife;

        if (instance == this)
            instance = null;
    }
}
