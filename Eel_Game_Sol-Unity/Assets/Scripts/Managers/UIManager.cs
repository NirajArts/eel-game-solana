using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject gameScreen;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public GameObject connectButtons;

    private GameManager gameManager;
    private GameStarter gameStarter;
    private ScoreManager scoreManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        gameStarter = GetComponent<GameStarter>();
        scoreManager = GetComponent<ScoreManager>();

        bestScoreText.text = "Best Score: 0";

        mainMenuScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(WalletState.Instance.IsConnected)
        {
            //connectButtons.SetActive(false);
        }
        else
        {
            //connectButtons.SetActive(true);
        }

        if (gameManager.isGameStarted)
        {
            mainMenuScreen.SetActive(false);
            gameScreen.SetActive(true);
            scoreText.text = "Score: " + scoreManager.Score.ToString("F0");
        }
    }
}
