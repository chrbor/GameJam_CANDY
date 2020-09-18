using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lässt das Objekt in einem festgesetzten Interval spawnen
/// </summary>
public class SpawnScript : MonoBehaviour
{
    /// <summary> Spawn- Dauer am Anfang </summary>
    public float startSpawnTime;
    /// <summary> Spawn- Dauer am Ende </summary>
    public float endSpawnTime;
    /// <summary> Zahl, um die sich die SpawnTime per Durchgang ändert? </summary>
    public float acceleration;
    /// <summary> Anzahl der Objekte, die per Durchgang gespawnt werden </summary>
    public float burstNumber;

    private bool spawnRunning;
    private float spawnTime;

    public GameObject spawn;
    private List<GameObject> objs = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(ContinousSpawn());
    }

    IEnumerator ContinousSpawn()
    {
        spawnTime = startSpawnTime;
        while (true)
        {
            objs.Clear();
            for(int i = 0; i < burstNumber; i++)
            {
                objs.Add(Instantiate(spawn, transform.position, Quaternion.identity));
            }
            yield return new WaitForSeconds(spawnTime);
            spawnTime -= acceleration;
            if (spawnTime > endSpawnTime) spawnTime = endSpawnTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, spawn.name, true);
    }
}
