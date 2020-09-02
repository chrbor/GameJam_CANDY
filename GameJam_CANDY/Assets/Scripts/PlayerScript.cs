using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class PlayerScript : CharScript
{
    private Animator anim;

    /// <summary> Bestimmt, wie schnell der Char bechleunigen kann </summary>
    public float acceleration = 0.2f;
    /// <summary> die maximale Geschwindigkeit, nit der sich der Char bewegen kann </summary>
    public float maxSpeed = 3f;
    /// <summary> gibt an wie stark der Char abbremst </summary>
    [Range(0f, 1f)]
    public float speedDamping;
    /// <summary> Bestimmt, wie hoch der Char springen kann </summary>
    public float jumpPower = 10f;

    
    /// <summary> Zähler, der bestimmt wie lange der Char noch springen kann </summary>
    private float jumpCounter;
    /// <summary> Wenn wahr, dann wird gerade eine Hechtrolle ausgeführt </summary>
    private bool blockDive;
    /// <summary> Wenn wahr, dann ist der Char in der Luft, wenn falsch, dann ist er auf dem Boden </summary>
    private bool inAir;
    /// <summary> Mask für den Raycast, um Bodenkontakt festzustellen </summary>
    private int rMask = (1<<10) | (1<<15);//Kontakt mit Ground und Einkaufswagen

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        jumpCounter = jumpPower;
    }

    // Update is called once per frame
    void Update()
    {
        //Checke ob in der Luft:
        inAir = !Physics2D.Raycast(transform.position, Vector2.down, 2.6f, rMask).collider;

        PlayerControl();
    }

    /// <summary>
    /// Steuerung des Spielers
    /// </summary>
    private void PlayerControl()
    {

        //Checke Hechtrolle:
        if ((Input.GetKey(KeyCode.Space) && !inAir) || blockDive)
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

        //Fallanimation:
        anim.SetBool("inAir", inAir);
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
        anim.SetInteger("dive", (int)Mathf.Sign(factor));
        anim.SetTrigger("doDive");
        float t = 0;
        while(t < 0.5f)
        {
            t += Time.fixedDeltaTime;
            rb.position += Vector2.right * factor * maxSpeed * 1.5f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        blockDive = false;
        yield break;
    }


    /// <summary>
    /// Trigger ist nur die Hitbox. Wenn die Hitbox ausgelöst wird, dann wird Schaden genommen
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageReturn dmgCaused = other.GetComponent<IDamageCausing>().CauseDamage(gameObject);

        lifepoints -= dmgCaused.damage;
        if (lifepoints <= 0) { manager.GameOver(); Play_Death(); }
    }

    public override void Play_Death()
    {
        Debug.Log("Player died");

    }
}
    