using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    /// <summary> Bestimmt, wie schnell der Char bechleunigen kann </summary>
    [Range(0f, 4f)]
    public float acceleration = 0.2f;
    /// <summary> die maximale Geschwindigkeit, nit der sich der Char bewegen kann </summary>
    [Range(1f, 10f)]
    public float maxSpeed = 3f;
    /// <summary> Bestimmt, wie hoch der Char springen kann </summary>
    [Range(1f, 20f)]
    public float jumpPower = 10f;

    
    /// <summary> Zähler, der bestimmt wie lange der Char noch springen kann </summary>
    private float jumpCounter;

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

        //Simple Steuerung des Spielers:

        //Checke Sprung:
        if(Input.GetKey(KeyCode.W) && jumpCounter > 0)
        {
            //Debug.Log("jump: " + jumpCounter);
            rb.velocity = jumpCounter == jumpPower ? new Vector2(rb.velocity.x, jumpCounter) : rb.velocity + Vector2.up * jumpCounter;
            jumpCounter--;
        }

        //Checke Bewegung:
        if(Input.GetKey(KeyCode.D) == Input.GetKey(KeyCode.A)) { Debug.Log("stop");  anim.SetBool("running", false); return; }

        anim.SetBool("running", true);
        float dir = Input.GetKey(KeyCode.D) ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, 1);

        if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            rb.velocity += Vector2.right * dir * acceleration;// * Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("Refill jumpPower");
        jumpCounter = jumpPower;
    }
}
