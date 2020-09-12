using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

public class PlayerScript : CharScript
{
    [HideInInspector]
    public Animator anim;
    private bool attacking;
    private bool eating;

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
    public GameObject hitbox;
    public GameObject interactionSymbol;

    /// <summary> Zähler, der bestimmt wie lange der Char noch springen kann </summary>
    private float jumpCounter;
    /// <summary> Wenn wahr, dann wird gerade eine Hechtrolle ausgeführt </summary>
    private bool blockDive;
    /// <summary> Wenn wahr, dann kann der Char nicht gesteuert werden </summary>
    private bool blockControl;
    /// <summary> Wenn wahr, dann ist der Char in der Luft, wenn falsch, dann ist er auf dem Boden </summary>
    private bool inAir;

    //Collect-Control:
    /// <summary> Wenn wahr, dann kann der Spieler keine Objekte aufsammeln/fallen lassen </summary>
    private bool blockDropCollect;
    /// <summary> Wenn wahr dann ist der Spieler in der Nähe eines Collectables und kann es aufnehmen </summary>
    private GameObject collectFocus;

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

        if (collectFocus)
        {
            interactionSymbol.transform.parent = null;
            interactionSymbol.transform.localScale = Vector3.one;
            interactionSymbol.transform.rotation = Quaternion.identity;
            interactionSymbol.SetActive(true);
            interactionSymbol.transform.position = (Vector2)(collectFocus.transform.position + 1 * Vector3.up);
        }
        else
        {
            interactionSymbol.transform.parent = transform;
            interactionSymbol.SetActive(false);
        }
    }

    public void ActivateControl() { blockControl = false; }

    public IEnumerator Eat(int addedHealth)
    {
        anim.SetBool("a_eat", true);
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("a_eat", false);
        lifepoints += addedHealth;
        if(addedHealth < 0)
        {
            StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake());
            StartCoroutine(gameMenu.HitOverlay());
        }

        if (lifepoints > 100) lifepoints = 100;
        if (lifepoints <= 0) Play_Death();
        gameMenu.SetHealth(lifepoints);

        eating = false;
        yield break;
    }

    /// <summary>
    /// Steuerung des Spielers
    /// </summary>
    private void PlayerControl()
    {
        anim.SetInteger("dive", (int)Mathf.Sign(transform.localScale.x/*rb.velocity.x*/));


        //Checke Hechtrolle:
        if ((Input.GetKey(KeyCode.P) && !inAir) || blockDive)
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
        if (Input.GetKey(KeyCode.Space) && weapon)
        {
            attacking = true;
            weapon.GetComponent<CollBase>().Fire();
            if (weapon.GetComponent<CollBase>().weapon.isProjectile) weapon = null;
        }

        //Checke ob Waffe fallen gelassen wird:
        if(Input.GetKey(KeyCode.E) && !blockDropCollect)
        {
            blockDropCollect = true;
            if (weapon) { weapon.GetComponent<CollBase>().Drop(); if (!collectFocus) StartCoroutine(gameMenu.HideWeaponHealth()); }
        }
        if (!Input.GetKey(KeyCode.E)) blockDropCollect = false;

        //Checke, ob waffe gegessen wird:
        if(Input.GetKey(KeyCode.H) && weapon && !eating)
        {
            eating = true;
            StartCoroutine(weapon.GetComponent<CollBase>().Eat());
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
        hitbox.SetActive(false);
        gameObject.layer = 9;//GroundOnly
        float t = 0;
        while(t < 0.3f)
        {
            t += Time.fixedDeltaTime;
            rb.position += Vector2.right * factor * maxSpeed * 1.5f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        hitbox.SetActive(true);
        gameObject.layer = 8;//Player
        blockDive = false;
        yield break;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy") || !other.GetComponent<CharScript>().active) return;//Trigger, um Candy zu aktivieren
        DamageReturn dmgCaused = other.GetComponent<IDamageCausing>().CauseDamage(gameObject);

        lifepoints -= dmgCaused.damage;
        anim.SetInteger("dive", (int)Mathf.Sign(dmgCaused.angle - 90));
        rb.AddForce(RotToVec(dmgCaused.angle) * dmgCaused.power);

        //blockControl = true;
        //StartCoroutine(PlayHit());

        //Spiele Animationen ab:
        StartCoroutine(Camera.main.GetComponent<CameraScript>().Shake());
        StartCoroutine(gameMenu.HitOverlay());
        gameMenu.SetHealth(lifepoints);
        if (lifepoints <= 0) { manager.GameOver(); Play_Death(); }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!(other.CompareTag("Collectable") || other.CompareTag("Finish"))) return;
        
        if (blockDropCollect || collectFocus) return;
        collectFocus = other.CompareTag("Finish")? other.gameObject : other.transform.parent.gameObject;
        StartCoroutine(Collect(collectFocus));
        return;      
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent && other.transform.parent.gameObject == collectFocus) collectFocus = null;
    }

    /// <summary>
    /// Fokust das Objekt, damit der Spieler es mit Tastendruck aufnehmen kann
    /// </summary>
    /// <param name="_weapon"></param>
    /// <returns></returns>
    IEnumerator Collect(GameObject _weapon)
    {
        GameObject currentFocus = collectFocus;
        yield return new WaitWhile(() => collectFocus == currentFocus && !Input.GetKey(KeyCode.E));

        if (collectFocus != currentFocus) yield break;
        if (collectFocus.CompareTag("Finish")) collectFocus.GetComponent<GoalScript>().EndLevel();

        yield return new WaitUntil(() => blockDropCollect);//Warte darauf die aktuelle Waffe fallen zu lassen

        weapon = _weapon.gameObject;

        CollBase coll = weapon.GetComponent<CollBase>();
        weapon.transform.parent = coll.weapon.leftHanded? left_Hand.transform : right_Hand.transform;
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
        sprite.sortingOrder = coll.weapon.leftHanded ? -2 : 4;

        coll.Highlight.SetActive(false);
        coll.Initialize();
        gameMenu.SetNewWeapon(weapon);
        StartCoroutine(gameMenu.ShowWeaponHealth());

        yield return new WaitUntil(() => !Input.GetKey(KeyCode.E));
        collectFocus = null;
        yield break;
    }

    IEnumerator PlayHit()
    {
        anim.SetInteger("hit", anim.GetInteger("hit") + 1);
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("hit", anim.GetInteger("hit") - 1);

        //Bypass Falling- Bug (needs to be fixed!)
        yield return new WaitForSeconds(3);
        ActivateControl();
        yield break;

    }

    public override void Play_Death()
    {
        Debug.Log("Player died and now needs a diet ;D");

    }
}
    