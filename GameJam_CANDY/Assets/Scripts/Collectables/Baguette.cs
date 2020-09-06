using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AdditionalTools;

/// <summary>
/// Das Baguette fungiert als Langschwert
/// </summary>
public class Baguette : CollBase
{
    public override void Fire()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy")) return;

        float x_diff = other.transform.position.x - transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0? -45:45)) * weapon.power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= weapon.damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }
}
