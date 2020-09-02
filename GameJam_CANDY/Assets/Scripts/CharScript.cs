using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basis aller Einheiten, die kämpfen können
/// </summary>
public abstract class CharScript : MonoBehaviour
{
    protected Rigidbody2D rb;
    /// <summary> Lebenspunkte der Einheit </summary>
    public float lifepoints;
    /// <summary> Die Waffe, die die Einheit aktuell besitzt </summary>
    public GameObject weapon;

    /// <summary> Animation vom Tod der Einheit </summary>
    public abstract void Play_Death();
}
