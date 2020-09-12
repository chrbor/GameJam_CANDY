using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AdditionalTools;

public class SingleCarrot : MonoBehaviour, IThrowableScript, ICaddyble
{
    public float power;
    public int damage;

    private bool destroyed;
    private Rigidbody2D rb;
    private Collider2D coll;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    public virtual void Throw()
    {
        StartCoroutine(Fly());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Candy"))
        {
            if (other.GetComponent<CharScript>().active) return;
            float x_diff = other.transform.position.x - transform.position.x;

            other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (rb.velocity.x > 0 ? -45 : 45)) * power);

            CharScript cScript = other.GetComponent<CharScript>();
            cScript.lifepoints -= damage;
            if (cScript.lifepoints < 0) cScript.Play_Death();
        }
        else
            destroyed = true;
    }


    IEnumerator Fly()
    {
        destroyed = false;
        for (float count = 0; !destroyed || count > 10f; count += Time.deltaTime)
        {
            //Richte dich nach der Flugrichtung aus (+50 als korrektur):
            rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg + 50;
            coll.enabled = count > 0.2f;
            yield return new WaitForEndOfFrame();
        }


        Destroy(GetComponent<Collider2D>());
        rb.velocity = new Vector2(-rb.velocity.x/4, Mathf.Abs(rb.velocity.y));
        float rotation = Random.Range(1f, 10f) * Mathf.Sign(Random.Range(-1f, 1f));
        for (float count = 0; count < 7f; count += Time.deltaTime)
        {
            rb.rotation += rotation;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield break;
    }

    public void FallIntoCaddy(Transform caddy)
    {
        GameObject obj = Instantiate(manager.caddyContent, caddy);
        obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        obj.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
