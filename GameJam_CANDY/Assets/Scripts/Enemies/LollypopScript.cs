using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AdditionalTools;

public class LollypopScript : CharScript, IDamageCausing
{
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
    public float attackSpeed;
    /// <summary> Zeit in Sekunden, bis der Lolly wieder angreifen kann </summary>
    public float attackCooldown;
    /// <summary> Geschwindigkeit, mit der sich der Lolly auf den Wagen ausrichtet </summary>
    public float rotationSpeed;
    /// <summary> Kraft, mit der der Lolly springt </summary>
    public float jumpPower;
    /// <summary> Zeit in Sekunden, bis der Lolly wieder springen kann </summary>
    public float jumpCooldown;

    //Temporary:
    public float jumpCorrection;

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

    private void Start()
    {
        inactiveScale = transform.localScale.x;
        active = true;

        anim = transform.GetChild(0).GetComponent<Animator>();
        face = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;//jepp, jeder einzelne bone D:
        Debug.Log(face.name);
    }

    void Update()
    {
        if (!run) return;

        diff = manager.player.transform.position - transform.position;
        diff_Caddy = manager.caddy.transform.position - transform.position;
        if (active)
        {
            Control();
        }
        else WaitForPlayer();
    }

    /// <summary>
    /// Warte darauf, dass der Spieler in die Nähe kommt
    /// </summary>
    private void WaitForPlayer()
    {
        //Do stuff
    }

    /// <summary>
    /// Steuert den Lollypop zum Einkaufswagen
    /// </summary>
    private void Control()
    {
        //Greife Spieler an, wenn in Reichweite:
        if(seePlayer && !cooldown_attack && !jumping)
        {
            if (attacking) return;
            attacking = true;
            StartCoroutine(Attack());
            return;
        }
        //Springe zum Caddy, wenn in Sichtweite
        if(seeCaddy && !cooldown_jump && !attacking)
        {
            if (!jumping) return;
            jumping = true;
            StartCoroutine(Jump());
            return;
        }

        anim.SetBool("Move", true);
        transform.localScale = new Vector3(diff_Caddy.x > 0 ? activeScale : -activeScale, activeScale, 1);
        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(rb.velocity.x) != Mathf.Sign(diff_Caddy.x))
            rb.velocity += Vector2.right * Mathf.Sign(diff_Caddy.x) * acceleration * Time.deltaTime;
    }

    IEnumerator Jump()
    {
        //Starte Jump-Animation:
        anim.SetBool("Move", false);
        anim.SetBool("Jump", true);

        //Richte dich während dessen zum Caddy aus:
        for (float count = 0; count < 0.5f; count += Time.deltaTime)
        {
            transform.localScale = new Vector3(diff_Caddy.x > 0 ? activeScale : -activeScale, activeScale, 1);
            rb.rotation += Mathf.Sign(Mathf.Atan2(diff.y + diff.sqrMagnitude/jumpCorrection, diff.x)) * rotationSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rb.AddForce(RotToVec(rb.rotation) * jumpPower);
        jumping = false;

        //Starte cooldown:
        cooldown_jump = true;
        for (float count = 0; count < jumpCooldown; count += Time.deltaTime) yield return new WaitForEndOfFrame();
        cooldown_jump = false;
        yield break;
    }

    IEnumerator Attack()
    {
        anim.SetBool("Move", false);
        anim.SetBool("Attack", true);

        Vector2 dir = diff.x > 0 ? Vector2.right : Vector2.left;
        for(float count = 0; count < 0.5f; count += Time.deltaTime)
        {
            transform.localScale = new Vector3(diff.x > 0 ? activeScale : -activeScale, activeScale, 1);
            rb.position += dir *  attackSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        attacking = false;

        //Starte cooldown:
        cooldown_attack = true;
        for (float count = 0; count < attackCooldown; count += Time.deltaTime) yield return new WaitForEndOfFrame();
        cooldown_attack = false;
        yield break;
    }



    public DamageReturn CauseDamage(GameObject enemy)
    {
        Vector2 diff = enemy.transform.position - transform.position;

        DamageReturn dmg = new DamageReturn(contactDamage, 90 + (diff.x > 0 ? -60 : 60), power);

        return dmg;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        //seePlayer = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //seePlayer = false;
    }
}
