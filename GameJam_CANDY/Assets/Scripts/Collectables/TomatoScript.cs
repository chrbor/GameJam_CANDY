using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AdditionalTools;

public class TomatoScript : CollBase
{
    private bool attacking;
    private bool exploding;

    public override void Fire()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(PlayAttack());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy")) return;

        Debug.Log("Hit");

        float x_diff = other.transform.position.x - transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0 ? -45 : 45)) * weapon.power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= weapon.damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameObject.layer != 19 || exploding) return;//nicht weapon
        exploding = true;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        StartCoroutine(PlayExplosion(transform.GetChild(0).GetComponent<SpriteRenderer>()));
    }

    IEnumerator PlayExplosion(SpriteRenderer sprite)
    {
        transform.localScale = Vector3.one * 3;
        sprite.sprite = Resources.LoadAll<Sprite>("Tomate")[1];
        GetComponent<Collider2D>().enabled = true;

        //float change;
        for (float count = 0; count < 0.5; count += Time.deltaTime)
        {
            //change = Time.deltaTime * 2;
            sprite.color -= Color.black * Time.deltaTime * 2;
            transform.localScale += Vector3.one * Time.deltaTime * 6;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield break;
    }

    IEnumerator PlayAttack()
    {
        Debug.Log("attack");

        manager.player.GetComponent<PlayerScript>().anim.SetTrigger(weapon.animTrigger);

        yield return new WaitForSeconds(0.3f);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.parent = null;
        transform.localScale = Vector3.one * display.size_inHand;
        GetComponent<CircleCollider2D>().radius = display.size_onGround;

        gameObject.layer = 19;//weapon
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(manager.player.transform.localScale.x * 1000, 500));
        yield return new WaitForSeconds(0.1f);
        GetComponent<CircleCollider2D>().enabled = true;


        float rotation = Random.Range(1f, 5f) * Mathf.Sign(Random.Range(-1f, 1f));
        while (!exploding)
        {
            transform.Rotate(Vector3.forward, rotation);
            yield return new WaitForEndOfFrame();
        }
        attacking = false;
        yield break;
    }
}
