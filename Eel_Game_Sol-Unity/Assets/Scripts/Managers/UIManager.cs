using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject gameScreen;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;

    private GameManager gameManager;
    private ScoreManager scoreManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        scoreManager = GetComponent<ScoreManager>();

        bestScoreText.text = "Best Score: 0";

        mainMenuScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.isGameStarted)
        {
            mainMenuScreen.SetActive(false);
            gameScreen.SetActive(true);
            scoreText.text = "Score: " + scoreManager.Score.ToString("F0");
        }
    }
}
