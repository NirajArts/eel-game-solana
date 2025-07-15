using System;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

/// <summary>
/// Manages in-game score (distance traveled) and submits to blockchain on death.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Game & Web3 Managers")]
    [Tooltip("Reference to the Web3ScoreManager to submit scores on-chain")]
    [SerializeField] private Web3ScoreManager web3ScoreManager;

    private Transform playerTransform;
    private float startZ;
    public float Score { get; private set; }

    void Start()
    {
        // Find the player by tag
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            startZ = playerTransform.position.z;
        }
        else
        {
            Debug.LogError("ScoreManager: No GameObject with tag 'Player' found in the scene.");
        }

        // If not set in inspector, try to find automatically
        if (web3ScoreManager == null)
        {
            web3ScoreManager = GetComponent<Web3ScoreManager>();
            if (web3ScoreManager == null)
                Debug.LogWarning("ScoreManager: Web3ScoreManager not assigned or found.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Score is the distance moved along Z from the starting position, always counting up
            Score = Mathf.Max(0f, playerTransform.position.z - startZ);
        }
    }

    /// <summary>
    /// Should be called when the player dies to submit the final score on-chain.
    /// </summary>
    public async void OnDeath()
    {
        Debug.Log("Player has died. Final Score: " + Score);

        if (web3ScoreManager != null)
        {
            uint finalScore = (uint)Mathf.Floor(Score);
            try
            {
                await web3ScoreManager.SubmitScoreAsync(finalScore);
                Debug.Log($"On-chain score submission succeeded: {finalScore}");
            }
            catch (Exception e)
            {
                Debug.LogError($"On-chain score submission failed: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot submit score: Web3ScoreManager reference is missing.");
        }
    }
}
