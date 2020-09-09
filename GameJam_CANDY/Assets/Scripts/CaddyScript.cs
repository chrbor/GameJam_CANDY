using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaddyScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private int rMask = (1 << 10);//Rycast soll nur Boden-> Wände treffen

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.1f) sprite.flipX = rb.velocity.x < 0;

        //Halte Platz nach Links und Rechts:
        RaycastHit2D hit_left = Physics2D.Raycast(transform.position, Vector2.left, 1f, rMask);
        RaycastHit2D hit_right = Physics2D.Raycast(transform.position, Vector2.right, 1f, rMask);
        if (hit_left.collider) rb.AddForce(Vector2.right * 5);
        if (hit_right.collider) rb.AddForce(Vector2.left * 5);
    }
}
