using UnityEngine;

public class TileManagerTarget : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    void Update()
    {
        if (!player) return;
        // Only follow if player is ahead in z
        if (player.position.z > transform.position.z)
        {
            float newZ = Mathf.Lerp(transform.position.z, player.position.z, smoothSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
        }
    }
}
