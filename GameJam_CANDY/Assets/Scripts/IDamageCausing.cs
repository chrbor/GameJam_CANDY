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
    public int damage;
    /// <summary> Die Kraftrichtung, mit der das getroffene Objekt zurückgeschleudert wird </summary>
    public float angle;
    /// <summary> Die Kraftmagnitude, mit der das getroffene Objekt zurückgeschleudert wird </summary>
    public float power;

    //Constructor

    /// <param name="_damage">Abzug der Lebenspunkte</param>
    /// <param name="_angle">Winkel, mit dem das gegnerische objekt zurückgeschleudert wird</param>
    /// <param name="_power">Kraftmagnitude, mit der das gegnerische Objekt zuurückgeschleudert wird</param>
    public DamageReturn(int _damage, float _angle, float _power)
    {
        damage = _damage;
        angle = _angle;
        power = _power;
    }
}
