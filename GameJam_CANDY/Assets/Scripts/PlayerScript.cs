using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AdditionalTools;

public class PlayerScript : CharScript
{
    private Animator anim;
    private bool attacking;

    [Header("Movement:")]
    /// <summary> Bestimmt, wie schnell der Char bechleunigen kann </summary>
    public float acceleration = 0.2f;
    /// <summary> die maximale Geschwindigkeit, nit der sich der Char bewegen kann </summary>
    public float maxSpeed = 3f;
    /// <summary> gibt an wie stark der Char abbremst </summary>
    [Range(0f, 1f)]
    public float speedDamping;
    /// <summary> Bestimmt, wie hoch der Char springen kann </summary>
    public float jumpPower = 10f;

    [Header("Hands:")]
    public GameObject left_Hand;
    public GameObject right_Hand;

    /// <summary> Zähler, der bestimmt wie lange der Char noch springen kann </summary>
    private float jumpCounter;
    /// <summary> Wenn wahr, dann wird gerade eine Hechtrolle ausgeführt </summary>
    private bool blockDive;
    /// <summary> Wenn wahr, dann kann der Char nicht gesteuert werden </summary>
    private bool blockControl;
    /// <summary> Wenn wahr, dann ist der Char in der Luft, wenn falsch, dann ist er auf dem Boden </summary>
    private bool inAir;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        jumpCounter = jumpPower;
    }


    // Update is called once per frame
    void Update()
    {
        //Checke ob in der Luft:
        inAir = !Physics2D.Raycast(transform.position, Vector2.down, 1.6f, rMask).collider;
        anim.SetBool("inAir", inAir);
        //anim.SetInteger("dive", (int)Mathf.Sign(rb.velocity.x));

        if (!run || blockControl) return;



        PlayerControl();
    }

    public void ActivateControl() { blockControl = false; }

    /// <summary>
    /// Steuerung des Spielers
    /// </summary>
    private void PlayerControl()
    {
        anim.SetInteger("dive", (int)Mathf.Sign(rb.velocity.x));


        //Checke Hechtrolle:
        if ((Input.GetKey(KeyCode.LeftShift) && !inAir) || blockDive)
        {
            jumpCounter = 0;
            if (!blockDive)
            {
                blockDive = true;
                float factor = Mathf.Sign(transform.localScale.x) * 1.5f;
                StartCoroutine(DoDive(factor));
            }
            return;
        }


        //Checke Sprung:
        if(Input.GetKey(KeyCode.W) && jumpCounter > 0)
        {
            //Debug.Log("jump: " + jumpCounter);
            rb.velocity = jumpCounter == jumpPower ? new Vector2(rb.velocity.x, jumpCounter) : rb.velocity + Vector2.up * jumpCounter;
            jumpCounter--;
        }
        

        //Checke Bewegung:
        if(Input.GetKey(KeyCode.D) == Input.GetKey(KeyCode.A))
        {
            rb.velocity *= new Vector2(1 - speedDamping ,1);
            anim.SetBool("running", false);
        }
        else
        {
            anim.SetBool("running", true);
            float dir = Input.GetKey(KeyCode.D) ? 1 : -1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, 1);

            if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(rb.velocity.x) != dir )
                rb.velocity += Vector2.right * dir * acceleration * Time.deltaTime;
        }


        //Checke Attacke:
        if (Input.GetKey(KeyCode.Space) && !attacking)
        {
            attacking = true;
            StartCoroutine(PlayAttack());
        }


        //Fallanimation:
        //anim.SetBool("inAir", inAir);
        if (inAir)
            anim.SetBool("falling", rb.velocity.y < 0);
        else
            jumpCounter = jumpPower;
    }

    /// <summary>
    /// Führt eine Hechtrolle in die richtung entsprechend factor aus
    /// </summary>
    /// <param name="factor">hechtrolle bei positiv, nach rechts und bei negativ, nach links</param>
    /// <returns></returns>
    private IEnumerator DoDive(float factor)
    {
        //anim.SetInteger("dive", (int)Mathf.Sign(factor));
        anim.SetTrigger("doDive");
        float t = 0;
        while(t < 0.25f)
        {
            t += Time.fixedDeltaTime;
            rb.position += Vector2.right * factor * maxSpeed * 1.5f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        blockDive = false;
        yield break;
    }


    /// <summary>
    /// Trigger ist nur die Hitbox. Wenn die Hitbox ausgelöst wird, dann wird entweder Schaden genommen oder etwas aufgesammelt
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectable"))
        {
            weapon = other.gameObject;

            CollBase coll = other.GetComponent<CollBase>();
            weapon.transform.parent = right_Hand.transform;
            weapon.transform.localScale = Vector3.one * coll.display.size_inHand;
            weapon.transform.localPosition = new Vector2(coll.display.handPos_x, coll.display.handPos_y);
            weapon.transform.eulerAngles = Vector3.forward * (right_Hand.transform.eulerAngles.z + Mathf.Sign(transform.localScale.x) * coll.display.handAngle);

            weapon.GetComponent<CircleCollider2D>().enabled = false;

            //Destroy(other.GetComponent<Rigidbody2D>());
            weapon.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            weapon.layer = 19;//Waffen- Layer

            coll.display.anim.SetBool("onGround", false);

            SpriteRenderer sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>();
            sprite.sortingLayerName = "Player";
            sprite.sortingOrder = 4;

            coll.light.SetActive(false);

            return;
        }

        if (!other.CompareTag("Candy")) return;//Trigger, um Candy zu aktivieren
        DamageReturn dmgCaused = other.GetComponent<IDamageCausing>().CauseDamage(gameObject);

        lifepoints -= dmgCaused.damage;
        anim.SetInteger("dive", (int)Mathf.Sign(dmgCaused.angle - 90));
        rb.AddForce(RotToVec(dmgCaused.angle) * dmgCaused.power);

        //blockControl = true;
        //StartCoroutine(PlayHit());
        if (lifepoints < 0) { manager.GameOver(); Play_Death(); }
    }

    IEnumerator PlayHit()
    {

        anim.SetInteger("hit", anim.GetInteger("hit") + 1);
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("hit", anim.GetInteger("hit") - 1);

        //Bypass Bug (needs to be fixed!)
        yield return new WaitForSeconds(3);
        ActivateControl();
        yield break;

    }

    IEnumerator PlayAttack()
    {
        Debug.Log("attack");
        Weapon _weapon = weapon.GetComponent<CollBase>().weapon;
        weapon.GetComponent<Collider2D>().enabled = true;

        anim.SetTrigger(_weapon.animTrigger);
        weapon.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        weapon.transform.GetChild(2).gameObject.SetActive(false);

        yield return new WaitForSeconds(_weapon.reload);
        weapon.GetComponent<Collider2D>().enabled = false;

        attacking = false;
        yield break;
    }

    public override void Play_Death()
    {
        Debug.Log("Player died");

    }
}
    