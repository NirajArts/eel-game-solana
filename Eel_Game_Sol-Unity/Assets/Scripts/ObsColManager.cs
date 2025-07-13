using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ObsColManager : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab for obstacles")]
    public GameObject obstaclePrefab;
    [Tooltip("Prefab for collectibles")]
    public GameObject collectiblePrefab;

    [Header("Spawn Range")]
    [Tooltip("Transform marking start of Z range (inclusive)")]
    public Transform rangeStart;
    [Tooltip("Transform marking end of Z range (inclusive)")]
    public Transform rangeEnd;

    [Header("Spawn Chances (0-10)")]
    [Range(0, 10)]
    [Tooltip("Chance weight for spawning an obstacle")]
    public int obstacleChance = 5;
    [Range(0, 10)]
    [Tooltip("Chance weight for spawning a collectible")]
    public int collectibleChance = 5;

    void Start()
    {
        TrySpawn();
    }

    void TrySpawn()
    {
        if (obstaclePrefab == null || collectiblePrefab == null)
            return;

        int totalChance = obstacleChance + collectibleChance;
        if (totalChance <= 0)
            return; // nothing spawns

        int pick = Random.Range(1, totalChance + 1);
        GameObject prefabToSpawn = null;

        // Determine which prefab to spawn
        if (pick <= obstacleChance)
            prefabToSpawn = obstacleChance > 0 ? obstaclePrefab : null;
        else
            prefabToSpawn = collectibleChance > 0 ? collectiblePrefab : null;

        if (prefabToSpawn == null)
            return;

        // Calculate random Z within given range
        float zWorld = Random.Range(rangeStart.position.z, rangeEnd.position.z);
        // Position relative to this tile
        Vector3 localPos = new Vector3(0f, 0f, zWorld - transform.position.z);

        // Instantiate as child and set local position
        GameObject go = Instantiate(prefabToSpawn);
        go.transform.SetParent(transform);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
    }
}
