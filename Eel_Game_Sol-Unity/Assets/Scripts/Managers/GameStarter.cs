using UnityEngine;

public class GameStarter : MonoBehaviour
{

    private PlayerManager playerManager;
    private Rigidbody playerRigidbody;
    private GameManager gameManager;
    private Animator playerAnimator;
    public bool allowPlay = false;
    public bool isGameStarted = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allowPlay = true;

        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        playerManager.enabled = false;
        playerRigidbody.isKinematic = true;

        gameManager = GetComponent<GameManager>();
        gameManager.isGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && allowPlay)
        {
            StartGame();
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
