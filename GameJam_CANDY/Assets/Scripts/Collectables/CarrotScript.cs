using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static AdditionalTools;

/// <summary>
/// Karotten werden geworfen und schlagen durch Gegner hindurch
/// </summary>
public class CarrotScript : CollBase
{
    private bool attacking;

    [Header("Carrot-spezifisch:")]
    public GameObject singleCarrot;
    private GameObject currentCarrot;

    public Vector2 single_HandPos;
    public float single_HandAngle;
    public float single_HandScale;
    public float single_GoundScale;
    public Vector2 throwForce;

    public override void Initialize()
    {
        SpawnCarrot();
    }

    public override void Fire()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(PlayAttack());
    }

    public override void GetDestroyed()
    {
        manager.player.GetComponent<PlayerScript>().weapon = null;

        StartCoroutine(gameMenu.HideWeaponHealth());
        Destroy(gameObject);
    }

    public override void Drop()
    {
        if(currentCarrot)
            Destroy(currentCarrot);
        base.Drop();
    }

    public override IEnumerator Eat()
    {
        if (!item.consumable) yield break;
        StartCoroutine(manager.player.GetComponent<PlayerScript>().Eat((int)(item.healthFactor * weapon.health)));
        yield return new WaitForSeconds(0.5f);
        manager.player.GetComponent<PlayerScript>().weapon = null;
        if (currentCarrot)
            Destroy(currentCarrot);
        Destroy(gameObject);
        yield break;
    }

    private void SpawnCarrot()
    {
        currentCarrot = Instantiate(singleCarrot, manager.player.GetComponent<PlayerScript>().right_Hand.transform);
        currentCarrot.transform.localPosition = single_HandPos;
        currentCarrot.transform.localRotation = Quaternion.Euler(0,0,single_HandAngle);
        currentCarrot.transform.localScale = Vector3.one * single_HandScale;
    }

    IEnumerator PlayAttack()
    {
        manager.player.GetComponent<PlayerScript>().anim.SetTrigger(weapon.animTrigger);
        
        if(!weapon.ignoreAnimation) yield return new WaitForSeconds(0.1f);
        if (manager.player.GetComponent<PlayerScript>().weapon != gameObject) yield break;
        currentCarrot.transform.GetChild(0).gameObject.SetActive(true);
        currentCarrot.transform.parent = null;
        currentCarrot.transform.localScale = Vector3.one * single_GoundScale;

        Rigidbody2D rb = currentCarrot.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(manager.player.transform.localScale.x * throwForce.x, throwForce.y));
        currentCarrot.GetComponent<IThrowableScript>().Throw();

        gameMenu.SetWeaponHealth(--weapon.health);
        if (weapon.health <= 0)
        {
            StartCoroutine(gameMenu.HideWeaponHealth());
            GetDestroyed();
        }
        else
        {
            yield return new WaitForSeconds(weapon.reload);
            if(manager.player.GetComponent<PlayerScript>().weapon == gameObject)
                SpawnCarrot();
        }

        attacking = false;
        yield break;
    }
}
