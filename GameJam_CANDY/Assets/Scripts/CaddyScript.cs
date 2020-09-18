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
        Sprite sprite = null;
        if (other.CompareTag("Candy"))
        {
            CharScript cScript = other.GetComponent<CharScript>();
            if (!cScript.active) return;

            sprite = cScript.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            gameMenu.SetCaddyHealth(--gameController.candyCount);
            Destroy(other.gameObject);
            if (gameController.candyCount <= 0) { Debug.Log("Zu viele Süßwaren im Einkaufswagen!"); gameMenu.GameOver(); }
        }

        else if (other.CompareTag("Collectable"))
        {
            if (other.transform.parent == manager.player.GetComponent<PlayerScript>().right_Hand.transform && other.gameObject.layer == 19) { Debug.Log("return"); return; }
            CollBase coll = other.transform.childCount > 0 ? other.GetComponent<CollBase>() : other.transform.parent.GetComponent<CollBase>();
            sprite = coll.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            int remainer = gameController.AddToCaddy(sprite.name, coll.weapon.leftHanded ? coll.weapon.health : 1);
            if (remainer == -1) return;

            if (remainer > 0) coll.weapon.health = remainer;
            else
            {
                if (coll.weapon.leftHanded) Destroy(manager.player.GetComponent<PlayerScript>().left_Hand.transform.GetChild(0).gameObject);
                Destroy(coll.gameObject);
            }
        }

        else if (other.CompareTag("Hitbox"))
        {
            sprite = other.GetComponent<SpriteRenderer>().sprite;
            int remainer = gameController.AddToCaddy(sprite.name, 1);
            if (remainer != 0) return;

            Destroy(other.gameObject);
        }
        else return;

        FallIntoCaddy(sprite);
    }

    private void FallIntoCaddy(Sprite sprite)
    {
        GameObject obj = Instantiate(manager.caddyContent, transform);
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.transform.localScale = transform.localScale;
    }
}
