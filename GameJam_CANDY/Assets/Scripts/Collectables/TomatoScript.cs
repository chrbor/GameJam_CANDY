using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

/// <summary>
/// Tomaten werden geworfen und explodieren, wenn sie aufschlagen
/// </summary>
public class TomatoScript : MonoBehaviour, IThrowableScript, ICaddyble
{
    public float power;
    public int damage;

    private bool exploding;

    public void Throw()
    {
        StartCoroutine(Fly());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy") || !other.GetComponent<CharScript>().active) return;

        Debug.Log("Hit");

        float x_diff = other.transform.position.x - transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0 ? -45 : 45)) * power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (exploding || other.gameObject.CompareTag("Caddy")) return;//nicht weapon
        exploding = true;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        StartCoroutine(PlayExplosion(GetComponent<SpriteRenderer>()));
    }

    IEnumerator PlayExplosion(SpriteRenderer sprite)
    {
        transform.localScale = Vector3.one * 2.5f;
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

    IEnumerator Fly()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<CircleCollider2D>().enabled = true;

        float rotation = Random.Range(1f, 5f) * Mathf.Sign(Random.Range(-1f, 1f));
        while (!exploding)
        {
            transform.Rotate(Vector3.forward, rotation);
            yield return new WaitForEndOfFrame();
        }
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
