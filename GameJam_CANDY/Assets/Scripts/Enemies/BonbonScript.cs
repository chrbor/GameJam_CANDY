using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static AdditionalTools;

public class BonbonScript : CharScript, IDamageCausing
{
    bool onGround;
    bool jumping;

    [Header("Bonbon-Settings:")]
    public float jumpAngle_min;
    public float jumpAngle_max;
    public float jumpPower_min;
    public float jumpPower_max;
    public float prepTime;

    /// <summary> Größe, des Bonbons, wenn aktiv </summary>
    public float activeScale;
    /// <summary> Größe des Bonbons, wenn inaktiv </summary>
    private float inactiveScale;

    private GameObject face;

    private void Start()
    {
        inactiveScale = transform.localScale.x;
        active = true;

        face = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (!run) return;
        
        if (active) JumpToCaddy();
        else WaitForPlayer();
    }

    /// <summary>
    /// Warte darauf, dass der Spieler in die Nähe kommt
    /// </summary>
    private void WaitForPlayer()
    {
        //Do stuff
    }

    /// <summary>
    /// Lässt den Bonbon zum wagen springen
    /// </summary>
    private void JumpToCaddy()
    {
        Vector2 diff = manager.caddy.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, rMask);
        onGround = hit.collider;
        if (!onGround) return;

        if(Mathf.Abs(diff.y) > 10f)
        {
            //Bewege dich zu einer Rampe, um in die entsprechende Ebene zu gelangen:
            return;
        }


        //Bewege dich zum Caddy:
        if (jumping) return;
        jumping = true;
        StartCoroutine(Jump(RotToVec(90 + Random.Range(jumpAngle_min, jumpAngle_max) * (diff.x < 0 ? 1 : -1)) * Random.Range(jumpPower_min, jumpPower_max), prepTime));

    }


    IEnumerator Jump(Vector2 puls, float prepTime)
    {
        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        float counter = 0;
        float percent;

        while(counter < prepTime)
        {
            //Falls Bonbon vom Platz geschubst wird, dann breche Jump ab
            if (!onGround || !active) { transform.localScale = Vector3.one * activeScale; jumping = false; coll.radius = 0.5f * activeScale; yield break; }

            //Stauche Sprite, um Antizipation aufzubauen:
            percent = 0.5f * counter / prepTime;
            transform.localScale = new Vector3(1+percent, 1-percent) * activeScale;
            coll.radius = 0.5f * (1 - 1.5f * percent);

            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;
        }

        rb.AddForce(puls);
        jumping = false;
        yield break;
    }

    public DamageReturn CauseDamage(GameObject enemy)
    {
        Vector2 diff = enemy.transform.position - transform.position;

        DamageReturn dmg = new DamageReturn(contactDamage, 90 + (diff.x > 0? -60:60), power);

        return dmg;
    }

    public override void Play_Death()
    {
        if (!active) return;
        active = false;
        StartCoroutine(Playing_Death());
    }

    IEnumerator Playing_Death()
    {
        float rotation = Random.Range(1f, 10f) * Mathf.Sign(Random.Range(-1f, 1f));
        transform.GetChild(2).gameObject.SetActive(true);

        for(float count = 0; count < 0.5f; count += Time.deltaTime)
        {
            transform.Rotate(Vector3.forward, rotation);
            yield return new WaitForEndOfFrame();
        }

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.Load<Sprite>("Wolke" + Random.Range(1, 5));
        Destroy(transform.GetChild(0).gameObject);
        GetComponent<CircleCollider2D>().enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        //yield return new WaitForSeconds(3);

        float change;
        for (float count = 0; count < 0.5f; count += Time.deltaTime)
        {
            change = Time.deltaTime * 2;
            sprite.color -= Color.black * change;
            transform.localScale += Vector3.one * change;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield break;
    }

}
