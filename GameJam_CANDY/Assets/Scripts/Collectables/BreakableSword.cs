using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

/// <summary>
/// Wie Baguette, emitiert jedoch zerbrochene Teile wenn zerstört
/// </summary>
public class BreakableSword : Baguette
{
    public GameObject[] parts;
    private List<GameObject> activeParts = new List<GameObject>();

    protected override IEnumerator PlayDestroy()
    {
        yield return new WaitUntil(()=>!attack_Action);
        Destroy(transform.GetChild(0).gameObject);

        foreach(var part in parts)
        {
            activeParts.Insert(0, Instantiate(part, transform.position, Quaternion.identity));

            activeParts[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-200, 200), Random.Range(500, 600)));
            activeParts[0].transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.forward);
            activeParts[0].GetComponent<Rigidbody2D>().AddTorque(Random.Range(-200, 200));
        }
        yield return new WaitForSeconds(7);

        foreach (var part in activeParts) Destroy(part);
        Destroy(gameObject);
        yield break;
    }
}
