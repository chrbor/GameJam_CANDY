using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaddyScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;


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
    }
}
