using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static WayPoint;
using static AdditionalTools;

public class LollypopScript : CharScript, IDamageCausing
{
    private static int pMask = (1 << 8) | (1 << 10) | (1 << 21);//Spieler oder Ground oder Slant
    private static int cMask = (1 << 15) | (1 << 10) | (1 << 21);//Caddy oder Ground oder Slant

    /// <summary> Größe, des Bonbons, wenn aktiv </summary>
    public float activeScale;
    /// <summary> Größe des Bonbons, wenn inaktiv </summary>
    private float inactiveScale;

    ///<summary> Geschwindigkeit, mit der sich der Lolly fortbewegt</summary>
    [Header("Lollypop- spezifisch:")]
    public float maxSpeed;
    /// <summary> Beschleunigung des Lollys </summary>
    public float acceleration;
    /// <summary> Geschwindigkeit, die der Lolly bei der Attacke hat </summary>
    public float attackRange;
    /// <summary> Zeit in Sekunden, bis der Lolly wieder angreifen kann </summary>
    public float attackCooldown;
    /// <summary> Geschwindigkeit, mit der sich der Lolly auf den Wagen ausrichtet </summary>
    public float rotationSpeed;
    /// <summary> Kraft, mit der der Lolly springt </summary>
    public float jumpPower;
    /// <summary> Zeit in Sekunden, bis der Lolly wieder springen kann </summary>
    public float jumpCooldown;

    /// <summary> Korrekturminderung, um vertikalbewegung mit einzuberechnen </summary>
    public float jumpCorrection;
    /// <summary> Distanz, auf die der Lollypop den Spieler und den Wagen erkennt </summary>
    public float viewRange;

    //AnimationStuff
    private GameObject face;
    private Animator anim;
    private bool seeCaddy;
    private bool seePlayer;
    private bool jumping;
    private bool cooldown_jump;
    private bool attacking;
    private bool cooldown_attack;

    //Helfer:
    Vector2 diff, diff_Caddy;
    private bool gettingActive;
    private bool inAir;

    private void Start()
    {
        inactiveScale = transform.localScale.x;

        anim = transform.GetChild(0).GetComponent<Animator>();
        face = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;//jepp, jeder einzelne bone D:
    }

    void Update()
    {
        if (!(run && active)) return;

        diff = manager.player.transform.position - transform.position;
        diff_Caddy = manager.caddy.transform.position - transform.position;

        Control();
    }

    /// <summary>
    /// Warte darauf, dass der Spieler in die Nähe kommt
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !(gettingActive || active))
        {
            gettingActive = true;
            StartCoroutine(GetActive());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == 10) inAir = false;//Ground getroffen?
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.layer == 10) inAir = true;//Ground getroffen?
    }


    IEnumerator GetActive()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 1.5f));
        anim.SetInteger("start", Random.Range(1, 4));
        face.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime) { transform.position += Vector3.up * Time.fixedDeltaTime/4; yield return new WaitForFixedUpdate(); }

        Destroy(GetComponent<BoxCollider2D>());
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        active = true;

        transform.localScale = Vector3.one * activeScale;
        yield break;
    }

    /// <summary>
    /// Steuert den Lollypop zum Einkaufswagen
    /// </summary>
    private void Control()
    {
        //Teste, ob der Lollypop den Spieler und/oder den Wagen sieht
        RaycastHit2D hit_Player = Physics2D.Raycast(transform.position, diff, viewRange, pMask);
        RaycastHit2D hit_Caddy = Physics2D.Raycast(transform.position, diff_Caddy, viewRange, cMask);
        seePlayer = hit_Player.collider && hit_Player.collider.CompareTag("Player");
        seeCaddy = hit_Caddy.collider && hit_Caddy.collider.CompareTag("Caddy");

        
        //Greife Spieler an, wenn in Reichweite:
        if(seePlayer && !(cooldown_attack || jumping || inAir))
        {
            if (attacking) return;
            attacking = true;
            StartCoroutine(Attack());
            return;
        }

        //Springe zum Caddy, wenn in Sichtweite
        if(seeCaddy && !(cooldown_jump || attacking || inAir))
        {
            if (jumping) return;
            jumping = true;
            StartCoroutine(Jump());
            return;
        }

        anim.SetBool("Move", true);
        if (Mathf.Abs(diff_Caddy.y) > 6)
        {
            //Bewege dich zu einer Rampe, um in die entsprechende Ebene zu gelangen:
            Vector2 waypoint = new Vector2();
            for (int i = 0; i < Waypoints.Count; i++)
            {
                waypoint = Waypoints[i];
                if (Mathf.Abs(waypoint.y - manager.caddy.transform.position.y) < 6) break;
            }
            diff_Caddy = waypoint - (Vector2)transform.position;
        }

        transform.localScale = new Vector3(diff_Caddy.x > 0 ? activeScale : -activeScale, activeScale, 1);
        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(rb.velocity.x) != Mathf.Sign(diff_Caddy.x))
            rb.velocity += Vector2.right * Mathf.Sign(diff_Caddy.x) * acceleration * Time.deltaTime;
    }

    IEnumerator Jump()
    {
        int dir_sign = (int)Mathf.Sign(diff_Caddy.x);

        anim.SetBool("Move", false);
        anim.SetBool("Jump", true);
        transform.localScale = new Vector3(dir_sign > 0 ? activeScale : -activeScale, activeScale, 1);
        rb.velocity = Vector2.zero;

        float rotation;
        //Richte dich während dessen zum Caddy aus:
        for (float count = 0; count < 1.75f; count += Time.fixedDeltaTime)
        {
            transform.localScale = new Vector3(dir_sign > 0 ? activeScale : -activeScale, activeScale, 1);
            rotation = Mathf.Atan2(diff_Caddy.y + 30 - diff_Caddy.sqrMagnitude/jumpCorrection, diff_Caddy.x) * Mathf.Rad2Deg - transform.eulerAngles.z - 90;
            while(Mathf.Abs(rotation) > 180) rotation -= Mathf.Sign(rotation) * 360;

            rb.rotation += Mathf.Sign(rotation) * rotationSpeed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(!inAir) rb.AddForce(RotToVec(rb.rotation + 90) * jumpPower);

        //Fluganimation:
        yield return new WaitForSeconds(0.1f);
        while(inAir) { rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90; yield return new WaitForEndOfFrame(); }
        rb.rotation = 0;

        anim.SetBool("Jump", false);
        jumping = false;

        //Starte cooldown:
        cooldown_jump = true;
        yield return new WaitForSeconds(jumpCooldown);
        cooldown_jump = false;
        yield break;
    }

    IEnumerator Attack()
    {
        int dir_sign = (int)Mathf.Sign(diff.x);

        anim.SetBool("Jump", true);        
        anim.SetInteger("dir", dir_sign);
        transform.localScale = new Vector3(dir_sign > 0 ? activeScale : -activeScale, activeScale, 1);
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        yield return new WaitForSeconds(1f);
        transform.localScale = Vector3.one * activeScale;
        anim.SetBool("Attack", true);
        anim.SetBool("Jump", false);

        Vector2 dir = dir_sign > 0 ? Vector2.right : Vector2.left;
        for(float count = 0; count < 0.5f; count += Time.fixedDeltaTime)
        {
            rb.position += dir *  attackRange * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("Attack", false);

        rb.bodyType = RigidbodyType2D.Dynamic;
        cooldown_attack = true;
        attacking = false;
        yield return new WaitForSeconds(attackCooldown);
        cooldown_attack = false;
        yield break;
    }

    public DamageReturn CauseDamage(GameObject enemy)
    {
        Vector2 diff = enemy.transform.position - transform.position;

        DamageReturn dmg = new DamageReturn(contactDamage, 90 + (diff.x > 0 ? -60 : 60), power);

        return dmg;
    }

    public override void Play_Death()
    {
        if (!active) return;
        active = false;
        StartCoroutine(Playing_Death());
    }

    IEnumerator Playing_Death()
    {
        yield return new WaitForEndOfFrame();
        transform.localScale = Vector3.one * activeScale;
        rb.bodyType = RigidbodyType2D.Dynamic;

        for (float count = 0; count < 0.5f || !inAir; count += Time.deltaTime)
        {
            rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg - 90;
            yield return new WaitForEndOfFrame();
        }

        SpriteRenderer sprite = anim.GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("Wolke" + Random.Range(1, 5));
        Destroy(face);
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;

        float change;
        for (float count = 0; count < 0.5f; count += Time.deltaTime)
        {
            change = Time.deltaTime * 2;
            sprite.color -= Color.black * change;
            transform.localScale += Vector3.one * change;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield break;
    }
}
