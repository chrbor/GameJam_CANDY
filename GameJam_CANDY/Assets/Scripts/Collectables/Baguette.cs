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


    public override void Fire()
    {
        if (attacking) return;
        hitting = false;
        attacking = true;
        StartCoroutine(PlayAttack());
    }

    public override void GetDestroyed()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;
    }

    public override void Drop()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * display.size_onGround;
        transform.GetChild(1).gameObject.SetActive(true);
        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        sprite.sortingLayerName = "Objects";
        sprite.sortingOrder = 0;
        manager.player.GetComponent<PlayerScript>().weapon = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = 11;//Object
        coll.radius = 1;
        coll.enabled = true;
    }

    public override void Eat()
    {
        if (item.consumable) StartCoroutine(manager.player.GetComponent<PlayerScript>().Eat((int)(item.healthFactor * weapon.health)));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Candy")) return;
        if (!hitting)
        {
            hitting = true;
            weapon.health--;
            if (weapon.health > 0)
                gameMenu.SetWeaponHealth(weapon.health);
            else GetDestroyed();
        }

        float x_diff = other.transform.position.x - transform.position.x;

        other.GetComponent<Rigidbody2D>().AddForce(RotToVec(90 + (x_diff > 0? -45:45)) * weapon.power);

        CharScript cScript = other.GetComponent<CharScript>();
        cScript.lifepoints -= weapon.damage;
        if (cScript.lifepoints < 0) cScript.Play_Death();
    }

    IEnumerator PlayAttack()
    {
        Debug.Log("attack");
        GetComponent<Collider2D>().enabled = true;

        manager.player.GetComponent<PlayerScript>().anim.SetTrigger(weapon.animTrigger);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        transform.GetChild(2).gameObject.SetActive(false);

        yield return new WaitForSeconds(weapon.reload);
        GetComponent<Collider2D>().enabled = false;

        attacking = false;
        hitting = false;
        yield break;
    }
}
