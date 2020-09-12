using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

/// <summary>
/// Das Baguette fungiert als Langschwert
/// </summary>
public class Baguette : CollBase
{
    private bool attacking;
    private bool hitting;
    protected bool attack_Action;

    public override void Fire()
    {
        if (attacking) return;
        hitting = false;
        attacking = true;
        attack_Action = true;
        StartCoroutine(PlayAttack());
    }

    public override void GetDestroyed()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;
        manager.player.GetComponent<PlayerScript>().weapon = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = 1;//Falle durch alles durch

        StartCoroutine(gameMenu.HideWeaponHealth());
        StartCoroutine(PlayDestroy());
    }

    public override void Drop()
    {
        base.Drop();
    }

    protected virtual IEnumerator PlayDestroy()
    {
        rb.AddForce(new Vector2(manager.player.transform.localScale.x * 100, 300));
        float rotation = Random.Range(1f, 10f) * Mathf.Sign(Random.Range(-1f, 1f));
        for (float count = 0; count < 3f; count += Time.deltaTime)
        {
            rb.rotation += rotation;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy") || !other.GetComponent<CharScript>().active) return;
        if (!hitting)
        {
            hitting = true;
            weapon.health--;
            if (weapon.health > 0)
                gameMenu.SetWeaponHealth(weapon.health);
            else GetDestroyed();
        }

        float x_diff = other.transform.position.x - manager.player.transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0? -45:45)) * weapon.power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= weapon.damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    IEnumerator PlayAttack()
    {
        GetComponent<Collider2D>().enabled = true;

        manager.player.GetComponent<PlayerScript>().anim.SetTrigger(weapon.animTrigger);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        transform.GetChild(2).gameObject.SetActive(false);

        attack_Action = false;
        yield return new WaitForSeconds(weapon.reload);
        GetComponent<Collider2D>().enabled = false;

        attacking = false;
        hitting = false;
        yield break;
    }
}
