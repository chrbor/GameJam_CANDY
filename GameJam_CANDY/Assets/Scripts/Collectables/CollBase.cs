using System.Collections;
using static GameManager;
using UnityEngine;

[System.Serializable]
public abstract class CollBase : MonoBehaviour, ICaddyble
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

    protected void Start()
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
        transform.parent = null;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * display.size_onGround;
        transform.GetChild(1).gameObject.SetActive(true);
        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Objects";
        sprite.sortingOrder = 0;
        manager.player.GetComponent<PlayerScript>().weapon = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = 11;//Object
        coll.radius = 0.5f;
        coll.enabled = true;
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
    public virtual IEnumerator Eat()
    {
        if (!item.consumable) yield break;
        StartCoroutine(manager.player.GetComponent<PlayerScript>().Eat((int)(item.healthFactor * weapon.health)));
        yield return new WaitForSeconds(0.5f);
        manager.player.GetComponent<PlayerScript>().weapon = null;
        Destroy(gameObject);
        yield break;
    }

    public virtual void Initialize()
    {
        Debug.Log("Initialize() nicht implementiert");
    }

    public virtual void FallIntoCaddy(Transform caddy)
    {
        GameObject obj = Instantiate(manager.caddyContent, caddy);
        obj.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        obj.transform.localScale = transform.localScale;
        Destroy(gameObject);
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
    /// <summary> Wenn wahr, dann liegt die Waffe in der linken Hand </summary>
    public bool leftHanded;
}

[System.Serializable]
public class Item
{
    /// <summary> Wenn wahr, dann kann das Objekt gegen Leben eingetauscht werden </summary>
    public bool consumable;
    /// <summary> Anzahl der Leben pro weapon.health, die der Spieler erhält, wenn er das Objekt konsumiert</summary>
    public float healthFactor;
}
