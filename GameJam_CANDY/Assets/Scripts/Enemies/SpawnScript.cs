using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public float spawnTime;
    public GameObject spawn;

    private void Start()
    {
        StartCoroutine(ContinousSpawn());
    }

    IEnumerator ContinousSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);
            Instantiate(spawn, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, spawn.GetComponent<SpriteRenderer>().sprite.name, true);
    }
}
