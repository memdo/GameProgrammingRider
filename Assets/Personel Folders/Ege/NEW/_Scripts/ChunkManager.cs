using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] chunkPrefabs; // Your array of chunks
    public Transform player;          // Drag your player here
    
    [Header("Settings")]
    public float chunkSize = 25f;    // Length of one chunk
    public float connectY = -23.5f;   // The fixed Y level
    public int chunksToKeep = 3;      // How many chunks visible at once
    
    // Internal tracking
    private float spawnX = 0f;
    private Queue<GameObject> activeChunks = new Queue<GameObject>();

    void Start()
    {
        // Safety check
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // 1. Spawn initial chunks so the player has ground immediately
        for (int i = 0; i < chunksToKeep + 1; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        if (player == null) return;

        // 2. Check distance. 
        // Logic: If player is past the point where we need a new chunk...
        // (spawnX - chunksToKeep * chunkSize) is roughly the end of the current generated path
        if (player.position.x > spawnX - (chunksToKeep * chunkSize))
        {
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        // A. Pick a random chunk
        int index = Random.Range(0, chunkPrefabs.Length);
        GameObject selectedPrefab = chunkPrefabs[index];

        // B. Calculate position (Start at current X, Fixed Y)
        Vector3 pos = new Vector3(spawnX, connectY, 0);

        // C. Spawn it
        GameObject newChunk = Instantiate(selectedPrefab, pos, Quaternion.identity);
        
        // D. Add to queue for tracking
        activeChunks.Enqueue(newChunk);

        // E. Advance the spawn pointer
        spawnX += chunkSize;

        // F. Cleanup old chunks
        if (activeChunks.Count > chunksToKeep + 2)
        {
            GameObject oldChunk = activeChunks.Dequeue();
            Destroy(oldChunk);
        }
    }
}