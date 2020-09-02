using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollBase : MonoBehaviour
{
    [Header("Anzeigewerte:")]
    /// <summary> Die Größe des Objektes wenn es in der Hand ist</summary>
    public float size_inHand;
    /// <summary> Die Größe des Objektes, wenn es auf dem Boden liegt </summary>
    public float size_onGround;
    /// <summary> Der Offset, mit dem das Objekt in der Hand liegt </summary>
    public Vector2 handPos;

    [Header("Waffenwerte:")]
    /// <summary> Schadenspunkte bei Treffer </summary>
    public float damage;
    /// <summary> Abklingzeit in Sekunden, bis die Waffe wieder verwendet werden kann </summary>
    public float reload;
    /// <summary> Krafteinwirkung auf Gegner bei Treffer </summary>
    public float power;
    /// <summary> Krafteinwirkung auf Spieler bei Fire() </summary>
    public float recoil;
    /// <summary> Wenn wahr, dann wird das Objekt geworfen</summary>
    public bool isProjectile;

    public void Fire()
    {
        Debug.Log("Waffe nicht implementiert");
    }


}
