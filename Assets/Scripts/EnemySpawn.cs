using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Your player car GameObject")]
    public Transform playerTransform;

    [Tooltip("Prefab(s) with a TrafficCar component — add as many sprite variants as you like")]
    public GameObject[] carPrefabs;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.8f;
    public int   maxCars       = 10;

    [Header("Spawn Positions")]
    [Tooltip("Extra units beyond the camera edge where cars spawn (keeps them out of view)")]
    public float spawnEdgePadding = 1.5f;

    [Header("Lane Clearance")]
    [Tooltip("Minimum Y distance between cars in the same lane. Car height is ~2 units, so 3-4 gives a safe buffer.")]
    public float minLaneClearance = 4f;

    [Header("Car Type Ratio")]
    [Range(0f, 1f)]
    [Tooltip("Probability a spawned car is a Faster overtaker (rest are Slower blockers)")]
    public float fasterChance = 0.5f;

    [Header("Player Speed Source")]
    [Tooltip("Assign your player movement script here — must implement ISpeedProvider")]
    public MonoBehaviour playerSpeedSource;

    private float            spawnTimer;
    private List<TrafficCar> activeCars = new List<TrafficCar>();
    private ISpeedProvider   speedProvider;
    private Camera           cam;

    void Start()
    {
        spawnTimer = spawnInterval;
        cam        = Camera.main;

        if (playerSpeedSource is ISpeedProvider sp)
            speedProvider = sp;
        else
            Debug.LogWarning("TrafficSpawner: playerSpeedSource does not implement ISpeedProvider.");
    }

    void Update()
    {
        if (playerTransform == null || carPrefabs == null || carPrefabs.Length == 0) return;

        float currentPlayerSpeed = speedProvider?.CurrentSpeed ?? 10f;

        activeCars.RemoveAll(c => c == null);
        foreach (var car in activeCars)
            car.playerSpeed = currentPlayerSpeed;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && activeCars.Count < maxCars)
        {
            spawnTimer = spawnInterval;
            SpawnCar(currentPlayerSpeed);
        }
    }

    void SpawnCar(float playerSpeed)
    {
        bool    isFaster  = Random.value < fasterChance;
        CarType type      = isFaster ? CarType.Faster : CarType.Slower;

        float camHalfH   = cam != null ? cam.orthographicSize : 6f;
        float camCenterY = cam != null ? cam.transform.position.y : playerTransform.position.y;
        float spawnY     = isFaster
            ? camCenterY - camHalfH - spawnEdgePadding
            : camCenterY + camHalfH + spawnEdgePadding;

        int playerLane       = GetPlayerLane();
        List<int> openLanes  = GetOpenLanes(playerLane, spawnY);

        if (openLanes.Count == 0)
            return;

        int   lane    = openLanes[Random.Range(0, openLanes.Count)];
        float spawnX  = LaneConfig.GetLaneX(lane);

        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject go     = Instantiate(prefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);

        TrafficCar car = go.GetComponent<TrafficCar>();
        if (car == null)
        {
            Debug.LogError("TrafficSpawner: prefab is missing a TrafficCar component!");
            Destroy(go);
            return;
        }

        car.carType     = type;
        car.playerSpeed = playerSpeed;
        activeCars.Add(car);
    }
    List<int> GetOpenLanes(int playerLane, float spawnY)
    {
        List<int> open = new List<int>();
        for (int i = 0; i < LaneConfig.LaneCount; i++)
        {
            if (i == playerLane) continue;
            if (IsLaneClear(i, spawnY))
                open.Add(i);
        }
        return open;
    }
    bool IsLaneClear(int laneIndex, float spawnY)
    {
        float laneX         = LaneConfig.GetLaneX(laneIndex);
        float halfLaneWidth = 1.5f;
        foreach (var car in activeCars)
        {
            if (car == null) continue;
            if (Mathf.Abs(car.transform.position.x - laneX) > halfLaneWidth) continue;
            if (Mathf.Abs(car.transform.position.y - spawnY) < minLaneClearance)
                return false;
        }
        return true;
    }

    int GetPlayerLane()
    {
        float px      = playerTransform.position.x;
        int   closest = 0;
        float closestDist = Mathf.Abs(px - LaneConfig.LaneX[0]);

        for (int i = 1; i < LaneConfig.LaneCount; i++)
        {
            float d = Mathf.Abs(px - LaneConfig.LaneX[i]);
            if (d < closestDist) { closestDist = d; closest = i; }
        }
        return closest;
    }
}