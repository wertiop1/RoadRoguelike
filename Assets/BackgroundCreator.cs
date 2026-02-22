using System.Collections.Generic;
using UnityEngine;

public class InfiniteRoadManager : MonoBehaviour
{
    public Transform player;              // Assign player in Inspector
    public GameObject roadPrefab;         // Assign prefab in Inspector
    public int numberOfSegments = 6;      // How many tiles to keep alive
    public float segmentLength = 20f;     // Length of one road tile

    private List<GameObject> segments = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        // Spawn initial road segments
        for (int i = 0; i < numberOfSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        // If player has moved forward enough
        if (player.position.z - 20f > segments[0].transform.position.z)
        {
            RecycleSegment();
        }
    }

    void SpawnSegment()
    {
        GameObject segment = Instantiate(roadPrefab, Vector3.forward * spawnZ, Quaternion.identity);
        segments.Add(segment);
        spawnZ += segmentLength;
    }

    void RecycleSegment()
    {
        GameObject oldSegment = segments[0];
        segments.RemoveAt(0);

        oldSegment.transform.position = Vector3.forward * spawnZ;
        segments.Add(oldSegment);

        spawnZ += segmentLength;
    }
}