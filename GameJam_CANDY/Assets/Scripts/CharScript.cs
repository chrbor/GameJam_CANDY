using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basis aller Einheiten, die kämpfen können
/// </summary>
public abstract class CharScript : MonoBehaviour
{
    protected Rigidbody2D rb;
    /// <summary> gibt an, ob das Objekt aktiv ist </summary>
    public bool active = true;
    /// <summary> Lebenspunkte der Einheit </summary>
    public int lifepoints;
    /// <summary> Schaden, der bei Kontakt dem Gegner zugefügt wird </summary>
    public int contactDamage;
    /// <summary> Die Kraft, die dem Gegner bei Kontakt zugefügt wird </summary>
    public float power;
    /// <summary> Die Waffe, die die Einheit aktuell besitzt </summary>
    public GameObject weapon;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary> Animation vom Tod der Einheit </summary>
    public virtual void Play_Death()
    {
        Debug.Log("Method 'Play_Death()' not implemented yet");
    }
}
