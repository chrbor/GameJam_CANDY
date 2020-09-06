using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class LollypopScript : CharScript, IDamageCausing
{
    /// <summary> Größe, des Bonbons, wenn aktiv </summary>
    public float activeScale;
    /// <summary> Größe des Bonbons, wenn inaktiv </summary>
    private float inactiveScale;

    private void Start()
    {
        inactiveScale = transform.localScale.x;
        active = true;
    }

    void Update()
    {
        if (!run) return;

        if (active) MoveToCaddy();
        else WaitForPlayer();
    }

    /// <summary>
    /// Warte darauf, dass der Spieler in die Nähe kommt
    /// </summary>
    private void WaitForPlayer()
    {
        //Do stuff
    }

    /// <summary>
    /// Steuert den Lollypop zum Einkaufswagen
    /// </summary>
    private void MoveToCaddy()
    {

    }


    public DamageReturn CauseDamage(GameObject enemy)
    {
        Vector2 diff = enemy.transform.position - transform.position;

        DamageReturn dmg = new DamageReturn(contactDamage, 90 + (diff.x > 0 ? -60 : 60), power);

        return dmg;
    }
}
