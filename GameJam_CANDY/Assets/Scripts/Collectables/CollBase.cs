using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CollBase : MonoBehaviour
{
    [SerializeField]
    public Coll_Display display;

    [SerializeField]
    public Weapon weapon;

    public GameObject light;

    private void Start()
    {
        display.anim = transform.GetChild(0).GetComponent<Animator>();
        //display.anim.SetBool("onGround", false);
    }

    /// <summary>
    /// Angriff der Waffe wird ausgeführt
    /// </summary>
    public virtual void Fire()
    {
        Debug.Log("'Fire()' nicht implementiert");
    }
}

[System.Serializable]
public class Coll_Display
{
    /// <summary> Die Größe des Objektes wenn es in der Hand ist</summary>
    public float size_inHand;
    /// <summary> Die Größe des Objektes, wenn es auf dem Boden liegt </summary>
    public float size_onGround;
    /// <summary> Der x- Offset, mit dem das Objekt in der Hand liegt </summary>
    public float handPos_x;
    /// <summary> Der y- Offset, mit dem das Objekt in der Hand liegt </summary>
    public float handPos_y;
    /// <summary> Der Winkel, mit dem das Objekt in der Hand liegt </summary>
    public float handAngle;
    /// <summary> Animator für das Objekt </summary>
    [HideInInspector]
    public Animator anim;
}

[System.Serializable]
public class Weapon
{
    /// <summary> Wenn wahr, dann wird das Objekt geworfen</summary>
    public bool isProjectile;

    /// <summary> Schadenspunkte bei Treffer </summary>
    public float damage;
    /// <summary> Abklingzeit in Sekunden, bis die Waffe wieder verwendet werden kann </summary>
    public float reload;
    /// <summary> Krafteinwirkung auf Gegner bei Treffer </summary>
    public float power;
    /// <summary> Krafteinwirkung auf Spieler bei Fire() </summary>
    public float recoil;
    /// <summary> Der Name des Triggers, der die Animation auslöst </summary>
    public string animTrigger;
    /// <summary> Die Anzahl an getroffenen Schlägen, bis die Waffe auseinanderbricht </summary>
    public float breakingPoint;
    /// <summary> aktuelle Zahl der getroffenen Schläge </summary>
    [HideInInspector]
    public float breakPointCount;
}
