using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private Transform playerTransform;
    private float startZ;
    public float Score { get; private set; }

    void Start()
    {
        // Find the player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            startZ = playerTransform.position.z;
        }
        else
        {
            Debug.LogError("ScoreManager: No GameObject with tag 'Player' found in the scene.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Score is the distance moved along Z from the starting position, always counting up from 0
            Score = Mathf.Max(0f, playerTransform.position.z - startZ);
        }
    }
}
