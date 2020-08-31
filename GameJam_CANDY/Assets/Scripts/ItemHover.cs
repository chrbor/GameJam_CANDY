using UnityEngine;
using System.Collections;
using System.Threading;

public class ItemHover : MonoBehaviour
{
    public Rigidbody2D rb2D;
    private Vector2 thrust;
    public float hight;
    public int thrustAm;

    void Start()
    {
        thrust.Set(0, thrustAm);
        hight = transform.position.y;
    }

    void FixedUpdate()
    {
       if (transform.position.y <= hight) 
        {
                 rb2D.AddForce(thrust);
        }
    }
}