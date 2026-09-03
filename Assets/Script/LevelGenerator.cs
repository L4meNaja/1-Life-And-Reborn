using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] corridorPrefabs;
    public GameObject[] roomPrefabs;

    public Transform spawnPoint;

    public void GenerateLevel()
    {
        // สุ่ม Corridor
        GameObject corridorPrefab =
            corridorPrefabs[Random.Range(0, corridorPrefabs.Length)];

        GameObject corridor =
            Instantiate(
                corridorPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        // หา RoomSpawnPoint ใน Corridor
        Transform roomSpawnPoint =
            corridor.transform.Find("RoomSpawnPoint");

        // สุ่ม Room
        GameObject roomPrefab =
            roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        Instantiate(
            roomPrefab,
            roomSpawnPoint.position,
            roomSpawnPoint.rotation
        );
    }
}