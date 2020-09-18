using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

/// <summary>
/// Melonen werden geworfen und rollen Gegner platt
/// </summary>
public class MelonScript : CollBase
{
    private bool attacking;

    public GameObject[] parts;
    private List<GameObject> activeParts = new List<GameObject>();

    public override void Fire()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(PlayAttack());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy") || !other.GetComponent<CharScript>().active) return;

        float x_diff = other.transform.position.x - transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0 ? -45 : 45)) * weapon.power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= weapon.damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    IEnumerator PlayAttack()
    {
        manager.player.GetComponent<PlayerScript>().anim.SetTrigger(weapon.animTrigger);

        yield return new WaitForSeconds(0.3f);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.parent = null;
        StartCoroutine(gameMenu.HideWeaponHealth());
        transform.localScale = Vector3.one * display.size_inHand * 2;

        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        coll.radius = display.size_inHand / 2;
        coll.isTrigger = true;
        coll.enabled = true;

        coll = transform.GetChild(3).GetComponent<CircleCollider2D>();
        coll.radius = display.size_inHand/2;

        gameObject.layer = 19;//weapon
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.drag = 0.1f;
        rb.AddForce(new Vector2(manager.player.transform.localScale.x * 6000, 500));
        yield return new WaitForSeconds(0.1f);
        coll.enabled = true;

        for (float count = 0; count < weapon.reload; count += Time.fixedDeltaTime)
        {
            transform.Rotate(Vector3.forward, -rb.velocity.x);
            yield return new WaitForFixedUpdate();
        }

        //Explosion:
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        transform.localScale = Vector3.one * 1.5f;
        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sprite.sprite = Resources.LoadAll<Sprite>("Tomate")[1];
        GetComponent<Collider2D>().enabled = true;

        foreach (var part in parts)
        {
            activeParts.Insert(0, Instantiate(part, transform.position, Quaternion.identity));

            activeParts[0].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-200, 200), Random.Range(500, 600)));
            activeParts[0].transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.forward);
            activeParts[0].GetComponent<Rigidbody2D>().AddTorque(Random.Range(-200, 200));
        }

        //float change;
        for (float count = 0; count < 0.5; count += Time.deltaTime)
        {
            //change = Time.deltaTime * 2;
            sprite.color -= Color.black * Time.deltaTime * 2;
            transform.localScale += Vector3.one * Time.deltaTime * 6;
            yield return new WaitForEndOfFrame();
        }
        Destroy(transform.GetChild(0).gameObject);
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(6);
        foreach (var part in activeParts) Destroy(part);
        Destroy(gameObject);
        attacking = false;
        yield break;
    }
}
