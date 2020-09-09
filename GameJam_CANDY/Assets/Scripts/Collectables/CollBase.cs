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

    [SerializeField]
    public Item item;

    public GameObject Highlight;
    protected Rigidbody2D rb;
    /// <summary> Collider, damit das Objekt nicht durch die Welt fällt </summary>
    protected CircleCollider2D coll;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        display.anim = transform.GetChild(0).GetComponent<Animator>();
        //display.anim.SetBool("onGround", false);

        weapon.health = weapon.maxHealth;
    }

    /// <summary>
    /// Angriff der Waffe wird ausgeführt
    /// </summary>
    public virtual void Fire()
    {
        Debug.Log("'Fire()' nicht implementiert");
    }

    /// <summary>
    /// Lässt die Waffe fallen
    /// </summary>
    public virtual void Drop()
    {
        Debug.Log("'Drop()' nicht implementiert");
    }

    /// <summary>
    /// Waffe wird zerstört
    /// </summary>
    public virtual void GetDestroyed()
    {
        Debug.Log("'GetDestroyed()' nicht implementiert");
    }

    /// <summary>
    /// Waffe wird gegessen
    /// </summary>
    public virtual void Eat()
    {
        Debug.Log("'Eat()' nicht implementiert");
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
    public int damage;
    /// <summary> Abklingzeit in Sekunden, bis die Waffe wieder verwendet werden kann </summary>
    public float reload;
    /// <summary> Krafteinwirkung auf Gegner bei Treffer </summary>
    public float power;
    /// <summary> Krafteinwirkung auf Spieler bei Fire() </summary>
    public float recoil;
    /// <summary> Der Name des Triggers, der die Animation auslöst </summary>
    public string animTrigger;
    /// <summary> Die maximale Anzahl an getroffenen Schlägen, die die Waffe aushält </summary>
    public int maxHealth;
    /// <summary> Die Anzahl an getroffenen Schlägen, bis die Waffe auseinanderbricht </summary>
    [HideInInspector]
    public int health;
}

[System.Serializable]
public class Item
{
    /// <summary> Wenn wahr, dann kann das Objekt gegen Leben eingetauscht werden </summary>
    public bool consumable;
    /// <summary> Anzahl der Leben pro weapon.health, die der Spieler erhält, wenn er das Objekt konsumiert</summary>
    public float healthFactor;
}
