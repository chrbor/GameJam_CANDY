using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Escalator : MonoBehaviour
{
    public float strength;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("Collectable") || other.CompareTag("Candy"))) return;
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (!rb) return;
        rb.AddForce(Vector2.up * strength * rb.mass);
    }
}

