using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AdditionalTools;
using static GameManager;

public class SingleToilet : MonoBehaviour, IThrowableScript
{
    public float power;
    public int damage;

    private bool destroyed;
    private Rigidbody2D rb;
    private EdgeCollider2D paperColl;
    private CircleCollider2D coll;

    public int paperLength;
    public int pointStep;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        paperColl = GetComponent<EdgeCollider2D>();
    }

    public void Throw()
    {
        //Korrektion der Flugrichtung
        transform.localScale = new Vector3(Mathf.Sign(manager.player.transform.localScale.x) * transform.localScale.x, transform.localScale.y, 1);
        StartCoroutine(Fly());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy") || !other.GetComponent<CharScript>().active) return;

        float x_diff = other.transform.position.x - transform.position.x;
        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (rb.velocity.x > 0 ? -45 : 45)) * power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Candy"))
        {
            if (!other.gameObject.GetComponent<CharScript>().active) return;
            float x_diff = other.transform.position.x - transform.position.x;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (rb.velocity.x > 0 ? -45 : 45)) * power);

            CharScript cScript = other.gameObject.GetComponent<CharScript>();
            cScript.lifepoints -= damage;
            if (cScript.lifepoints < 0) cScript.Play_Death();
        }
        else
            destroyed = true;
    }

    public void FallIntoCaddy(Transform caddy)
    {
        GameObject obj = Instantiate(manager.caddyContent, caddy);
        obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        obj.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }

    IEnumerator Fly()
    {
        destroyed = false;
        int stepCount = 1;
        Vector2 prevPos = transform.position;
        Vector2 diffPos;
        bool reverse = transform.localScale.x < 0;

        List<Vector2> pos_List = new List<Vector2>();
        //float rotation = Random.Range(1f, 5f) * Mathf.Sign(Random.Range(-1f, 1f));
        transform.rotation = Quaternion.identity;
        for (float count = 0; !destroyed || count > 10f; count += Time.fixedDeltaTime)
        {
            coll.enabled = count > 0.2f;
            //Aktualisiere das Kollisionsband:
            if(stepCount % pointStep == 0)
            {
                if (pos_List.Count == paperLength) pos_List.RemoveAt(0);

                diffPos = prevPos - (Vector2)transform.position;
                if (reverse) diffPos.x = -diffPos.x;

                prevPos = transform.position;
                for (int i = 0; i < pos_List.Count; i++) pos_List[i] += diffPos;

                pos_List.Add(Vector2.zero);
                paperColl.points = pos_List.ToArray();
            }

            stepCount++;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Destroyed");
        Destroy(paperColl);
        Destroy(coll);
        rb.velocity = new Vector2(-rb.velocity.x / 4, Mathf.Abs(rb.velocity.y));
        float rotation = Random.Range(1f, 10f) * Mathf.Sign(Random.Range(-1f, 1f));
        for (float count = 0; count < 7f; count += Time.deltaTime)
        {
            rb.rotation += rotation;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield break;
    }
}
