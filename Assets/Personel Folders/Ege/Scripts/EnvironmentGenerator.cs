using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

public class EnvironmentGenerator : MonoBehaviour
{
    // --- Public Fields from Unity Asset (HCR Bike.unity) ---
    public SpriteShapeController _spriteShapeController; // fileID: 5225404
    public int _levelLength = 100; // Total segments to generate in a full level
    public float _xMultiplier = 15.9f; // Horizontal spacing between points
    public float _yMultiplier = 24f; // Vertical scaling factor
    public float _curveSmoothness = 0.1f; // Controls tangent strength/curve density
    public int _bottom = 10; // Lowest point of the mesh for colliders

    // --- REVISED: Coin Spawning Fields for Density and Limits ---
    public GameObject coinPrefab; 
    private const float COIN_SPAWN_HEIGHT = -14.5f;        // Vertical offset (kept small, as previously fixed)
    private const float COIN_SPAWN_CHANCE = 0.7f;        // NEW: Increased probability (70%) for higher density
    private const int COIN_SPAWN_INTERVAL = 1;          // NEW: Check every point for maximum density
    private const int MAX_CONSECUTIVE_COINS = 4;        // NEW: Limit sequential spawns to max 4
    
    // --- Fields for Generation Logic ---
    private Spline spline;
    private int pointCount;
    private float generationDistance = 0f; 
    private const float GENERATION_TRIGGER_DISTANCE = 50f; 

    void Start()
    {
        spline = _spriteShapeController.spline;
        
        ClearSpline(); 
        
        // Initial generation
        GenerateGround(true); 
    }
    
    // --- Continuous Generation Trigger ---
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerVehicleTransform != null)
        {
            float playerX = GameManager.Instance.playerVehicleTransform.position.x;
            
            if (playerX + GENERATION_TRIGGER_DISTANCE > generationDistance)
            {
                GenerateGround(false); 
            }
        }
    }

    private void ClearSpline()
    {
        while (spline.GetPointCount() > 4)
        {
            spline.RemovePointAt(3); 
        }
        pointCount = spline.GetPointCount(); 
    }

    /// <summary>
    /// Generates a new batch of ground segments and triggers coin spawning.
    /// </summary>
    private void GenerateGround(bool isFirstRun)
    {
        Vector3 startPosition = spline.GetPosition(pointCount - 3);
        int segmentsPerBatch = isFirstRun ? _levelLength : 10; 
        
        int initialPointCount = pointCount; 

        for (int i = 0; i < segmentsPerBatch; i++)
        {
            Vector3 newPointPos = new Vector3(
                startPosition.x + _xMultiplier * (i + 1),
                (Mathf.PerlinNoise(0, startPosition.x * 0.1f + i * 0.1f) - 0.5f) * _yMultiplier, 
                0
            );

            spline.InsertPointAt(pointCount - 2, newPointPos);
            
            spline.SetTangentMode(pointCount - 2, ShapeTangentMode.Continuous);
            spline.SetLeftTangent(pointCount - 2, Vector3.left * _xMultiplier * _curveSmoothness);
            spline.SetRightTangent(pointCount - 2, Vector3.right * _xMultiplier * _curveSmoothness);

            pointCount++;
        }

        generationDistance = spline.GetPosition(pointCount - 3).x; 

        _spriteShapeController.BakeMesh();

        // Pass the starting index for coin spawning
        SpawnCoins(initialPointCount);
    }

    /// <summary>
    /// Spawns coins along the newly generated terrain points, limited to a max sequence.
    /// </summary>
    private void SpawnCoins(int startIndex)
    {
        if (coinPrefab == null)
        {
            Debug.LogError("Coin Prefab is not assigned to the EnvironmentGenerator!");
            return;
        }

        // NEW: Counter to enforce the maximum sequence limit
        int consecutiveCoins = 0;

        // Iterate through the newly added control points, stopping before the closing floor points
        for (int i = startIndex; i < pointCount - 2; i++)
        {
            // Check interval (now 1, meaning check every point for density)
            if (i % COIN_SPAWN_INTERVAL != 0)
            {
                consecutiveCoins = 0; // Reset counter if we skipped this point
                continue;
            }

            bool shouldSpawn = false;
            
            if (consecutiveCoins < MAX_CONSECUTIVE_COINS)
            {
                // Only attempt to spawn if we haven't hit the limit
                if (Random.value < COIN_SPAWN_CHANCE)
                {
                    shouldSpawn = true;
                }
            }

            if (shouldSpawn)
            {
                Vector3 pointPosition = spline.GetPosition(i);

                Vector3 spawnPosition = new Vector3(
                    pointPosition.x, 
                    pointPosition.y + COIN_SPAWN_HEIGHT, 
                    0
                );

                Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
                consecutiveCoins++;
            }
            else
            {
                // If we hit the max consecutive limit OR failed the random check, reset the counter
                consecutiveCoins = 0;
            }
        }
    }
}