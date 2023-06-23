using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 10f;
    public float spawnInterval = 1f;
    public int entitiesPerSpawn = 1;

    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEntities();
            timer = spawnInterval;
        }
    }

    private void SpawnEntities()
    {
        for (int i = 0; i < entitiesPerSpawn; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject prefab = GetRandomPrefab();

            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float halfWidth = spawnAreaWidth / 2f;
        float halfHeight = spawnAreaHeight / 2f;

        float randomX = Random.Range(-halfWidth, halfWidth);
        float randomZ = Random.Range(-halfHeight, halfHeight);

        Vector3 spawnPosition = new Vector3(randomX, 0, randomZ) + this.transform.position;
        return spawnPosition;
    }

    private GameObject GetRandomPrefab()
    {
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        return prefabsToSpawn[randomIndex];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaWidth, 0f, spawnAreaHeight));
    }
}
