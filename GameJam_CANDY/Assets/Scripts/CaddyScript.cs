using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static GameController;

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
        if (hit_left.collider) rb.AddForce(Vector2.right * 10);
        if (hit_right.collider) rb.AddForce(Vector2.left * 10);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Candy-> Damage, Collectable-> liegende Sachen, Hitbox-> Wurfgeschosse
        //if (!(other.CompareTag("Candy") || other.CompareTag("Collectable") || other.CompareTag("Hitbox"))) return;

        if (other.CompareTag("Candy"))
        {
            if (!other.GetComponent<CharScript>().active) return;
            other.GetComponent<ICaddyble>().FallIntoCaddy(transform);
            gameMenu.SetCaddyHealth(--gameController.candyCount);
            return;
        }

        if (other.CompareTag("Collectable"))
        {
            if (other.transform.parent == manager.player.GetComponent<PlayerScript>().right_Hand) return;
            CollBase coll = other.transform.parent? other.transform.parent.GetComponent<CollBase>() : other.GetComponent<CollBase>();
            Sprite sprite = coll.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            int remainer = gameController.AddToCaddy(sprite.name, coll.weapon.leftHanded ? coll.weapon.health : 1);
            if (remainer == -1) return;

            if(remainer > 0) coll.weapon.health = remainer; 
            FallIntoCaddy(sprite);

            return;
        }

        if (other.CompareTag("Hitbox"))
        {
            Sprite sprite = other.GetComponent<SpriteRenderer>().sprite;
            int remainer = gameController.AddToCaddy(sprite.name, 1);
            if (remainer != 0) return;

            FallIntoCaddy(sprite);

            return;
        }
    }

    private void FallIntoCaddy(Sprite sprite)
    {
        GameObject obj = Instantiate(manager.caddyContent, transform);
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
