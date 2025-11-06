using UnityEngine;

/// <summary>
/// Manages in-game score (distance traveled) and submits to blockchain on death.
/// </summary>
public class ScoreManager : MonoBehaviour
{
//    [SerializeField] private AnchorScoreManager anchorScoreManager; // Reference to the AnchorScoreManager script

    private Transform playerTransform;
    private float startZ;
    public float Score { get; private set; }    bool isPlayerRegistered = false;
    void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            startZ = playerTransform.position.z;
        }
        else Debug.LogError("ScoreManager: No GameObject with tag 'Player'.");
        
        // if (anchorScoreManager==null)
        // {
        //     anchorScoreManager = GetComponent<AnchorScoreManager>();
        //     if (anchorScoreManager==null)
        //         Debug.LogWarning("ScoreManager: AnchorScoreManager not assigned or found.");
        // }
    }



    async void Update()
    {
        if (playerTransform != null)
            Score = Mathf.Max(0f, playerTransform.position.z - startZ);

    }

    public async void OnDeath()     // Called when player dies (e.g. from playerStats script)
    {
        if (playerTransform == null)
        {
            Debug.LogError("ScoreManager: Player transform is null on death.");
            return;
        }

        if (!isPlayerRegistered)
        {
            Debug.LogWarning("ScoreManager: Player not registered, cannot save score.");
            return;
        }

        // Save the score to the blockchain
        // var result = await scoreService.SaveScore((uint)Score);
        // if (result != null && result.WasSuccessful)
        // {
        //     Debug.Log($"Score saved successfully: {result.Result}");
        // }
    }
}