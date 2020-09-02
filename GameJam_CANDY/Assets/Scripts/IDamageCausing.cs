using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface aller Dinge, die dem Spieler Schaden zufügen
/// </summary>
public interface IDamageCausing
{
    /// <summary>
    /// Füge dem angegebenen Target Schaden zu
    /// </summary>
    /// <param name="target">Das Objekt, das getroffen wurde</param>
    /// <returns></returns>
    DamageReturn CauseDamage(GameObject target);
}

public class DamageReturn
{
    /// <summary> Der Schaden, der zugefügt wurde </summary>
    public float damage;
    /// <summary> Die Kraft, mit der das getroffene Objekt zurückgeschleudert wird </summary>
    public Vector2 power;

    //Constructor
    DamageReturn(float _damage, Vector2 _power)
    {
        damage = _damage;
        power = _power;
    }
}
