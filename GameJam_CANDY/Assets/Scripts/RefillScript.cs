using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lässt ein Objekt spawnen, sobald das ursprüngliche Objekt seinen spawn-Punkt verlassen hat
/// </summary>
public class RefillScript : MonoBehaviour
{
    /// <summary> Cooldown- Zeit </summary>
    public float refillTime;
    public GameObject spawn;
    private GameObject obj;

    public string icon;

    // Start is called before the first frame update
    void Start()
    {
        obj = Instantiate(spawn, transform.position, Quaternion.identity, null);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(obj == other.gameObject) { Debug.Log("object taken"); obj = null; StartCoroutine(Refill()); }
    }

    private IEnumerator Refill()
    {
        Debug.Log("refilling");
        yield return new WaitForSeconds(refillTime);
        obj = Instantiate(spawn, transform.position, Quaternion.identity, null);
        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, icon, false);
        if (spawn) name = spawn.name + "Spawn";
    }
}
