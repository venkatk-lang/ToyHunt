using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereSpawner : MonoBehaviour
{
    public GameObject spherePrefab;
    public float spawnInterval = 2f;

    private int totalSpawned = 0;
    private const int maxToSpawn = 20;
    private const int maxActive = 10;

    private List<GameObject> activeSpheres = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (totalSpawned < maxToSpawn)
        {
            // Pause if active limit reached
            if (activeSpheres.Count < maxActive)
            {
                SpawnSphere();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                // Wait until a sphere gets destroyed
                yield return null;
            }
        }
    }

    void SpawnSphere()
    {
        Vector3 pos = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity);
        activeSpheres.Add(sphere);

        // Assign script for destruction
        sphere.GetComponent<SphereClickDestroy>().Init(this);

        totalSpawned++;
    }

    // Called from sphere script when destroyed
    public void RemoveSphere(GameObject sphere)
    {
        activeSpheres.Remove(sphere);
    }
}