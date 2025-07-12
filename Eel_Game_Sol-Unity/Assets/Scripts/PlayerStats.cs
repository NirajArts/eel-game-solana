using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Animator animator; // Reference to Animator
    public PlayerManager playerManager; // Reference to movement script
    public Rigidbody[] ragdollRigidbodies; // All ragdoll rigidbodies
    public Collider[] ragdollColliders; // All ragdoll colliders
    public Collider mainCollider; // Main player collider
    public Rigidbody playerRigidbody; // Reference to player's Rigidbody
    public CameraFollow cameraFollow; // Reference to camera follow script

    void Start()
    {
        SetRagdollState(false);
    }

    public void Die()
    {
        // Disable animation and movement
        if (animator) animator.enabled = false;
        if (playerManager) playerManager.enabled = false;
        if (cameraFollow) cameraFollow.enabled = false;
        // Stop and freeze player Rigidbody
        if (playerRigidbody)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }
        // Enable ragdoll
        SetRagdollState(true);
    }

    void SetRagdollState(bool state)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
        }
        foreach (var col in ragdollColliders)
        {
            col.enabled = state;
        }
        if (mainCollider) mainCollider.enabled = !state;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Obstacle")) {
            Die();
        }
    }
} 