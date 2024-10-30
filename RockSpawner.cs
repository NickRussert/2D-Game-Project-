using UnityEngine;
using System.Collections.Generic;

public class RockSpawner : MonoBehaviour
{
    public GameObject[] rockPrefabs;       // Array of different rock prefabs
    public float initialSpawnInterval = 3f; // Initial spawn interval
    public float minSpawnInterval = 1f;     // Minimum interval between spawns

    private bool hasGameStarted = false;    // Track if the game has started
    private GameObject initialRock;         // Reference to the first rock
    private float nextSpawnTime;            // Track when the next rock should spawn
    private Queue<GameObject> rockQueue = new Queue<GameObject>();  // Queue to track rock order

    void Start()
    {
        // Spawn the first rock immediately
        SpawnInitialRock();
    }

    void Update()
    {
        // Start the game on player input and enable the first rock’s movement
        if (!hasGameStarted && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            hasGameStarted = true;
            EnableRockMovement(initialRock);  // Enable the first rock’s movement
        }

        // Spawn new rocks continuously after the game has started
        if (hasGameStarted && Time.time >= nextSpawnTime)
        {
            SpawnRock();
        }
    }

    void SpawnInitialRock()
    {
        // Random X position within -5f to 5f range
        float xPosition = Random.Range(-5f, 5f);
        Vector3 spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize - 1f, 0);

        // Randomly select a rock prefab from the array
        int randomIndex = Random.Range(0, rockPrefabs.Length);
        initialRock = Instantiate(rockPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        initialRock.AddComponent<RockMovement>().enabled = false;  // Disable movement initially

        rockQueue.Enqueue(initialRock);  // Add the first rock to the queue
    }

    void EnableRockMovement(GameObject rock)
    {
        RockMovement rockMovement = rock.GetComponent<RockMovement>();
        if (rockMovement != null) rockMovement.enabled = true;

        nextSpawnTime = Time.time + GetDynamicSpawnInterval();
    }

    void SpawnRock()
    {
        // Random X position within -5f to 5f range
        float xPosition = Random.Range(-5f, 5f);
        Vector3 spawnPosition = new Vector3(xPosition, Camera.main.orthographicSize + 1f, 0);

        // Randomly select a rock prefab from the array
        int randomIndex = Random.Range(0, rockPrefabs.Length);
        GameObject newRock = Instantiate(rockPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        newRock.AddComponent<RockMovement>().enabled = true;  // Enable movement

        rockQueue.Enqueue(newRock);  // Add the new rock to the queue

        nextSpawnTime = Time.time + GetDynamicSpawnInterval();  // Schedule the next spawn
    }

    public GameObject GetNextRock()
    {
        return rockQueue.Count > 0 ? rockQueue.Peek() : null;  // Return the next rock if available
    }

    public void DequeueNextRock()
    {
        if (rockQueue.Count > 0) rockQueue.Dequeue();  // Remove the rock from the queue
    }

    float GetDynamicSpawnInterval()
    {
        // Gradually decrease spawn interval over time for increasing difficulty
        return Mathf.Clamp(3f - Time.timeSinceLevelLoad / 30f, minSpawnInterval, initialSpawnInterval);
    }
}
