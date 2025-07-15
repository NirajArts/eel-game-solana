using UnityEngine;

public class GameStarter : MonoBehaviour
{

    private PlayerManager playerManager;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;
    private Animator playerAnimator;
    public bool allowPlay = false;
    public bool isGameStarted = false;
    private PlayerStats playerStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerManager.enabled = false;
        playerRigidbody.isKinematic = true;

        gameManager = GetComponent<GameManager>();
        gameManager.isGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (allowPlay && !isGameStarted && playerStats.playerDead == false)
                StartGame();

            else
            {
                Debug.Log("Connect Wallet to play the game.");
            }
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        playerManager.enabled = true;
        playerRigidbody.isKinematic = false;
        gameManager.isGameStarted = true;
        playerAnimator.SetTrigger("Run");
    }
}
