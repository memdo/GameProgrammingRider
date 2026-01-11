using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [Header("Setup")]
    // IMPORANT: Put prefabs here in the EXACT SAME ORDER as your Menu "bikes" array
    public GameObject[] vehiclePrefabs; 
    public Transform spawnPoint;

    void Awake()
    {
        // 1. Read the saved choice (Default to 0 if nothing saved)
        int selectedIndex = PlayerPrefs.GetInt("selectedBike", 0);

        // 2. Safety check: If saved index is larger than array, reset to 0
        if (selectedIndex >= vehiclePrefabs.Length) 
        {
            selectedIndex = 0;
        }

        // 3. Determine spawn position
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : transform.position;

        // 4. Spawn the vehicle
        GameObject playerObj = Instantiate(vehiclePrefabs[selectedIndex], pos, Quaternion.identity);

        // 5. CRITICAL: Ensure the spawned object is tagged "Player"
        // (Ideally, set this in the Prefab itself, but this is a safety measure)
        playerObj.tag = "Player";
    }
}