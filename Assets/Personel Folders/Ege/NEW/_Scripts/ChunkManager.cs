using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] chunkPrefabs;     // Your array of random chunks
    public GameObject bottomTerrainPrefab; // DRAG YOUR 'taban75' PREFAB HERE!
    public GameObject startWallPrefab;    // --- DRAG YOUR WALL PREFAB HERE ---
    public Transform player;              // Drag your player here

    [Header("Settings")]
    public float chunkSize = 75f;     // Length of one chunk
    public float connectY = -23.5f;   // The fixed Y level
    public int chunksToKeep = 3;      // How many chunks visible at once

    [Header("Progressive Difficulty Settings")]
    [Tooltip("Distance intervals where difficulty increases (e.g., every 300 units)")]
    public float difficultyIncreaseInterval = 300f;

    [Tooltip("Starting difficulty level (0 = easiest, higher = harder)")]
    public float baseDifficultyLevel = 0f;

    [Tooltip("Maximum difficulty level cap")]
    public float maxDifficultyLevel = 5f;

    [Tooltip("How quickly difficulty ramps up (higher = faster increase)")]
    public float difficultyRampSpeed = 1.0f;
    
    [Tooltip("If true, automatically detects chunk difficulty from prefab names (Easy, Medium, Hard)")]
    public bool autoDetectDifficulty = true;
    
    [Tooltip("Manual difficulty weights for chunks (if auto-detect is false). Index matches chunkPrefabs array.")]
    public int[] manualChunkDifficulties;

    bool first = true;

    // Internal tracking
    private float spawnX = 0f;
    private Queue<GameObject> activeChunks = new Queue<GameObject>();
    
    // Progressive difficulty tracking
    private float currentDifficultyLevel = 0f;
    private int[] chunkDifficultyLevels; // Stores difficulty level for each chunk prefab
    
    // Difficulty category tracking for active chunks
    private Dictionary<GameObject, int> chunkDifficultyMap = new Dictionary<GameObject, int>(); // Maps chunk GameObject to its difficulty category (0=Easy, 1=Medium, 2=Hard)
    private int easyCount = 0;
    private int mediumCount = 0;
    private int hardCount = 0;

    void Start()
    {
        // Safety check
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Initialize chunk difficulty levels
        InitializeChunkDifficulties();

        // 1. Spawn initial chunks so the player has ground immediately
        for (int i = 0; i < chunksToKeep + 1; i++)
        {
            SpawnChunk();
        }
        
        // Ensure counts are accurate and print initial difficulty counts
        RecalculateDifficultyCounts();
        PrintDifficultyCounts();
    }

    void Update()
    {
        if (player == null) return;
        
        // Update difficulty based on player progress
        UpdateDifficultyLevel();

        // 2. Check distance. 
        if (player.position.x > spawnX - (chunksToKeep * chunkSize))
        {
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        if (first)
        {
            Vector3 posa = new Vector3(spawnX, connectY - 3.6f, 0);
            GameObject newChunka = Instantiate(bottomTerrainPrefab, posa, Quaternion.identity);

            // --- ADDED TASK: Spawn Wall at -22 (Vertical) ---
            if (startWallPrefab != null)
            {
                // Instantiate at x = -22
                Vector3 wallPos = new Vector3(-19f, 0 , 0);
                
                // ROTATION: Quaternion.Euler(0, 0, 90) makes it VERTICAL
                GameObject wall = Instantiate(startWallPrefab, wallPos, Quaternion.Euler(0, 0, 90));
                
                // Make it a child of the first chunk so it deletes automatically
                wall.transform.SetParent(newChunka.transform);
            }
            // ------------------------------------------------

            activeChunks.Enqueue(newChunka);
            first = false;
            spawnX += chunkSize;
            return;
        }

        // A. Pick a chunk based on progressive difficulty (Top Layer)
        int index = SelectChunkByDifficulty();
        GameObject selectedPrefab = chunkPrefabs[index];
        
        // Get the difficulty category for this chunk (0=Easy, 1=Medium, 2=Hard)
        int difficultyCategory = GetDifficultyCategory(chunkDifficultyLevels[index]);

        // B. Calculate position for Top Layer
        Vector3 pos = new Vector3(spawnX, connectY, 0);

        // C. Spawn Top Layer
        GameObject newChunk = Instantiate(selectedPrefab, pos, Quaternion.identity);
        
        // Track this chunk's difficulty
        chunkDifficultyMap[newChunk] = difficultyCategory;
        UpdateDifficultyCount(difficultyCategory, 1);

        // --- NEW HARDCODED LOGIC FOR BOTTOM TERRAIN ---

        if (bottomTerrainPrefab != null)
        {
            // Calculate position: Same X, but Y is subtracted by 4
            Vector3 bottomPos = new Vector3(spawnX, connectY - 3.6f, 0);

            // Spawn the bottom part
            GameObject bottomChunk = Instantiate(bottomTerrainPrefab, bottomPos, Quaternion.identity);

            // CRITICAL: Make the bottom chunk a child of the top chunk.
            // This ensures that when 'newChunk' is destroyed later, 'bottomChunk' is deleted with it automatically.
            bottomChunk.transform.SetParent(newChunk.transform);
        }

        // ----------------------------------------------

        // D. Add the Top Layer (parent) to queue for tracking
        activeChunks.Enqueue(newChunk);

        // E. Advance the spawn pointer
        spawnX += chunkSize;

        // F. Cleanup old chunks
        if (activeChunks.Count > chunksToKeep + 2)
        {
            GameObject oldChunk = activeChunks.Dequeue();
            
            // Remove difficulty tracking for deleted chunk
            if (chunkDifficultyMap.ContainsKey(oldChunk))
            {
                int removedDifficultyCategory = chunkDifficultyMap[oldChunk];
                UpdateDifficultyCount(removedDifficultyCategory, -1);
                chunkDifficultyMap.Remove(oldChunk);
            }

            Destroy(oldChunk); // This deletes the top chunk AND the bottom child attached to it
        }
    }
    
    /// <summary>
    /// Initializes chunk difficulty levels either by auto-detection or manual assignment
    /// </summary>
    private void InitializeChunkDifficulties()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogWarning("ChunkManager: No chunk prefabs assigned!");
            return;
        }
        
        chunkDifficultyLevels = new int[chunkPrefabs.Length];
        
        if (autoDetectDifficulty)
        {
            // Auto-detect difficulty from prefab names
            for (int i = 0; i < chunkPrefabs.Length; i++)
            {
                if (chunkPrefabs[i] == null) continue;
                
                string prefabName = chunkPrefabs[i].name.ToLower();
                
                // Detect difficulty from common naming patterns
                if (prefabName.Contains("easy") || prefabName.Contains("simple") || prefabName.Contains("straight"))
                {
                    chunkDifficultyLevels[i] = 0; // Easy
                }
                else if (prefabName.Contains("medium") || prefabName.Contains("normal") || prefabName.Contains("medium"))
                {
                    chunkDifficultyLevels[i] = 2; // Medium
                }
                else if (prefabName.Contains("hard") || prefabName.Contains("difficult") || prefabName.Contains("extreme"))
                {
                    chunkDifficultyLevels[i] = 4; // Hard
                }
                else
                {
                    // Default to medium difficulty if name doesn't match patterns
                    chunkDifficultyLevels[i] = 2;
                }
            }
        }
        else
        {
            // Use manual difficulty assignments
            if (manualChunkDifficulties != null && manualChunkDifficulties.Length == chunkPrefabs.Length)
            {
                chunkDifficultyLevels = manualChunkDifficulties;
            }
            else
            {
                Debug.LogWarning("ChunkManager: Manual difficulty array doesn't match chunk prefabs count. Using defaults.");
                for (int i = 0; i < chunkDifficultyLevels.Length; i++)
                {
                    chunkDifficultyLevels[i] = 2; // Default to medium
                }
            }
        }
    }
    
    /// <summary>
    /// Updates the current difficulty level based on player progress
    /// </summary>
    private void UpdateDifficultyLevel()
    {
        if (player == null || GameManager.Instance == null) return;
        
        // Get player's distance traveled (using MaxDistanceTraveled from GameManager)
        float distanceTraveled = GameManager.Instance.MaxDistanceTraveled;
        
        // Calculate difficulty level based on distance
        // Difficulty increases gradually as player progresses
        float rawDifficulty = baseDifficultyLevel + (distanceTraveled / difficultyIncreaseInterval) * difficultyRampSpeed;
        
        // Clamp to max difficulty
        currentDifficultyLevel = Mathf.Clamp(rawDifficulty, baseDifficultyLevel, maxDifficultyLevel);
    }
    
    /// <summary>
    /// Selects a chunk index based on current difficulty level using weighted random selection
    /// </summary>
    private int SelectChunkByDifficulty()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogError("ChunkManager: No chunk prefabs available!");
            return 0;
        }

        if (chunkDifficultyLevels == null || chunkDifficultyLevels.Length != chunkPrefabs.Length)
        {
            // Fallback to random selection if difficulties aren't initialized
            return Random.Range(0, chunkPrefabs.Length);
        }

        // Calculate weights for each chunk based on difficulty level
        float[] weights = new float[chunkPrefabs.Length];
        float totalWeight = 0f;

        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            if (chunkPrefabs[i] == null) continue;

            int chunkDifficulty = chunkDifficultyLevels[i];
            int chunkCategory = GetDifficultyCategory(chunkDifficulty);

            // Progressive difficulty with randomness:
            // - Easy chunks: Always available with decreasing frequency
            // - Medium chunks: Start appearing around difficulty level 0.3+
            // - Hard chunks: Start appearing around difficulty level 1.0+
            // All with random variations to prevent predictable patterns

            float weight = 0f;

            // Calculate base weight with randomness for more natural progression
            if (chunkCategory == 0) // Easy chunks
            {
                // Easy chunks are always available but their weight decreases as difficulty increases
                float baseWeight = Mathf.Max(0.15f, 1.0f - (currentDifficultyLevel * 0.2f));
                // Add some randomness - sometimes easy chunks can appear even at higher difficulties
                float randomFactor = Random.Range(0.7f, 1.3f);
                weight = baseWeight * randomFactor;
            }
            else if (chunkCategory == 1) // Medium chunks
            {
                // Medium chunks start appearing when difficulty reaches 0.3 (earlier than before)
                // They become more common as difficulty increases
                if (currentDifficultyLevel >= 0.3f)
                {
                    float mediumProgression = Mathf.Clamp01((currentDifficultyLevel - 0.3f) / 2.0f);
                    float baseWeight = 0.25f + (mediumProgression * 1.0f);
                    // Add randomness - medium chunks can sometimes appear earlier or later
                    float randomFactor = Random.Range(0.8f, 1.4f);
                    weight = baseWeight * randomFactor;
                }
                else
                {
                    // Small random chance at very early difficulty levels
                    weight = Random.value * 0.1f;
                }
            }
            else if (chunkCategory == 2) // Hard chunks
            {
                // Hard chunks start appearing when difficulty reaches 1.0 (earlier than before)
                // They become more common as difficulty increases further
                if (currentDifficultyLevel >= 1.0f)
                {
                    float hardProgression = Mathf.Clamp01((currentDifficultyLevel - 1.0f) / 2.5f);
                    float baseWeight = hardProgression * 1.5f;
                    // Add randomness - hard chunks can sometimes appear earlier
                    float randomFactor = Random.Range(0.9f, 1.6f);
                    weight = baseWeight * randomFactor;
                }
                else
                {
                    // Very small random chance before difficulty 1.0
                    weight = Random.value * 0.05f;
                }
            }

            // Additional difficulty-based weighting with some randomness
            if (weight > 0)
            {
                float difficultyDifference = Mathf.Abs(chunkDifficulty - currentDifficultyLevel);

                // Apply exponential decay but with some randomness
                float decayFactor = Mathf.Exp(-difficultyDifference * Random.Range(0.5f, 0.9f));
                weight *= decayFactor;

                // Random boost for variety - sometimes chunks outside optimal range get selected
                if (Random.value < 0.15f) // 15% chance
                {
                    weight *= Random.Range(1.2f, 1.8f);
                }
                // Normal boost for chunks near current difficulty
                else if (chunkDifficulty <= currentDifficultyLevel + 1.5f && chunkDifficulty >= currentDifficultyLevel - 0.5f)
                {
                    weight *= Random.Range(1.1f, 1.5f);
                }
            }

            weights[i] = Mathf.Max(0, weight);
            totalWeight += weights[i];
        }

        // If no weights were assigned (shouldn't happen), fallback to random
        if (totalWeight <= 0)
        {
            return Random.Range(0, chunkPrefabs.Length);
        }

        // Normalize weights
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] /= totalWeight;
        }

        // Add some global randomness - occasionally allow "unexpected" chunk selections
        // This prevents the generation from being too predictable
        if (Random.value < 0.08f) // 8% chance for completely random selection
        {
            return Random.Range(0, chunkPrefabs.Length);
        }

        // Weighted random selection with slight randomization
        float randomValue = Random.value;
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return i;
            }
        }

        // Fallback (shouldn't reach here)
        return Random.Range(0, chunkPrefabs.Length);
    }
    
    /// <summary>
    /// Converts chunk difficulty level to category (Easy: 0, Medium: 1, Hard: 2)
    /// </summary>
    private int GetDifficultyCategory(int difficultyLevel)
    {
        // Map difficulty levels to categories:
        // 0 = Easy
        // 1-3 = Medium  
        // 4+ = Hard
        if (difficultyLevel <= 0)
        {
            return 0; // Easy
        }
        else if (difficultyLevel <= 3)
        {
            return 1; // Medium
        }
        else
        {
            return 2; // Hard
        }
    }
    
    /// <summary>
    /// Updates the difficulty count and prints the current status
    /// </summary>
    private void UpdateDifficultyCount(int category, int change)
    {
        switch (category)
        {
            case 0: // Easy
                easyCount += change;
                break;
            case 1: // Medium
                mediumCount += change;
                break;
            case 2: // Hard
                hardCount += change;
                break;
        }

        // Verify and correct counts by recounting active chunks
        RecalculateDifficultyCounts();

        // Print the current difficulty counts
        PrintDifficultyCounts();
    }

    /// <summary>
    /// Recalculates difficulty counts from active chunks to ensure accuracy
    /// </summary>
    private void RecalculateDifficultyCounts()
    {
        easyCount = 0;
        mediumCount = 0;
        hardCount = 0;

        // Clean up any null references in the dictionary
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var kvp in chunkDifficultyMap)
        {
            if (kvp.Key == null)
            {
                toRemove.Add(kvp.Key);
            }
            else
            {
                switch (kvp.Value)
                {
                    case 0: easyCount++; break;
                    case 1: mediumCount++; break;
                    case 2: hardCount++; break;
                }
            }
        }

        // Remove null references
        foreach (var key in toRemove)
        {
            chunkDifficultyMap.Remove(key);
        }
    }
    
    /// <summary>
    /// Prints the current difficulty counts in the format "Easy: X Medium: Y Hard: Z"
    /// </summary>
    private void PrintDifficultyCounts()
    {
        Debug.Log($"Easy: {easyCount} Medium: {mediumCount} Hard: {hardCount}");
    }
}