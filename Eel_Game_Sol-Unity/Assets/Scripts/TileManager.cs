using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [Tooltip("All tile prefabs for different difficulty segments")]
    public GameObject[] tilePrefabs;

    [Header("Player Settings")]
    [Tooltip("Reference to the player's transform")]
    public Transform playerTransform;

    [Tooltip("How far ahead of the player (in Z) to spawn new tiles")]
    public float safeZone = 25f;

    [Header("Tile Settings")]
    [Tooltip("Length of each tile along Z-axis")]
    public float tileLength = 20f;

    [Tooltip("Number of tiles to keep on screen")]
    public int amnTilesOnScreen = 2;

    [Tooltip("Horizontal offset for spawning tiles relative to player's X")]
    public float spawnXOffset = 0f;

    [Tooltip("Z offset for spawning tiles relative to base spawn Z")]
    public float spawnZOffset = 0f;

    private float spawnZ;               // Z position for next base spawn
    private float maxPlayerZ;           // Furthest Z the player has reached
    private List<GameObject> activeTiles;
    private int lastPrefabIndex = -1;

    // Difficulty control indices
    private int imFirst = 0;
    private int imLast = 5;

    void Start()
    {
        activeTiles = new List<GameObject>();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize spawnZ at player's starting Z
        spawnZ = playerTransform.position.z;
        maxPlayerZ = spawnZ;

        // Pre-spawn tiles
        for (int i = 0; i < amnTilesOnScreen; i++)
        {
            if (i == 0)
                SpawnTile(0); // Start tile
            else
                SpawnTile(Random.Range(1, tilePrefabs.Length)); // Random normal tile
        }
    }

    void Update()
    {
        // Track the furthest Z the player has moved
        if (playerTransform.position.z > maxPlayerZ)
            maxPlayerZ = playerTransform.position.z;

        // Spawn and remove based on safeZone
        if (maxPlayerZ + safeZone > spawnZ - amnTilesOnScreen * tileLength)
        {
            // Adjust difficulty ranges based on progress
        //    if (maxPlayerZ < 50f)
        //    {
        //        imFirst = 0; imLast = 4;
        //    }

            imFirst = 0;
            imLast = tilePrefabs.Length;

            // Spawn new tile
            SpawnTile(Random.Range(imFirst, imLast));
            DeleteTiles();
        }
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;
        if (prefabIndex < 0)
            go = Instantiate(tilePrefabs[RandomPrefabIndex()]);
        else
            go = Instantiate(tilePrefabs[prefabIndex]);

        go.transform.SetParent(transform);

        // Compute spawn position with static offsets
        float xPos = playerTransform.position.x + spawnXOffset;
        float zPos = spawnZ + spawnZOffset;
        go.transform.position = new Vector3(xPos, transform.position.y, zPos);
        go.transform.rotation = transform.rotation;

        // Move base spawnZ forward
        spawnZ += tileLength;
        activeTiles.Add(go);
    }

    private void DeleteTiles()
    {
        if (activeTiles.Count == 0)
            return;

        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
        if (tilePrefabs.Length <= 1)
            return 0;

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
            randomIndex = Random.Range(0, tilePrefabs.Length);

        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
}
