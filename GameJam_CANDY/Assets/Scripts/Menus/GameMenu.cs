using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MenuScript
{
    public static GameMenu gameMenu;

    private GameObject hud;
    private Slider healthbar;
    private GameObject weaponHealth;
    private Text weaponText;
    private Image weaponImage;

    private bool stopRoutine;

    // Start is called before the first frame update
    void Start()
    {
        gameMenu = this;

        hud = transform.Find("HUD").gameObject;
        healthbar = hud.transform.GetChild(0).GetComponent<Slider>();
        healthbar.value = 100;
        weaponHealth = hud.transform.GetChild(1).gameObject;
        weaponText = weaponHealth.transform.GetChild(1).GetComponent<Text>();
        weaponImage = weaponHealth.transform.GetChild(0).GetComponent<Image>();

        weaponHealth.SetActive(false);
    }

    /// <summary>
    /// Aktualisiert die Lebensleiste
    /// </summary>
    /// <param name="healthbar_left"></param>
    public void SetHealth(int healthbar_left)
    {
        healthbar.value = healthbar_left;
        if (healthbar_left <= 0) StartCoroutine(HideWeaponHealth());

        //Hier der Code um die Leiste langsam zu dem neuen Wert übergehen zu lassen P:
    }

    /// <summary>
    /// Ändert die Anzeige zur neuen Waffe
    /// </summary>
    /// <param name="sprite"></param>
    public void SetNewWeapon(GameObject weapon)
    {
        weaponImage.sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        weaponHealth.SetActive(true);
        SetWeaponHealth(weapon.GetComponent<CollBase>().weapon.health);
    }

    public void SetWeaponHealth(int health)
    {
        weaponText.text = ": " + health.ToString();
    }

    public IEnumerator ShowWeaponHealth()
    {
        weaponHealth.gameObject.SetActive(false);
        stopRoutine = true;
        yield return new WaitForEndOfFrame();
        stopRoutine = false;
        weaponHealth.gameObject.SetActive(true);


        CanvasGroup _weaponHealth = weaponHealth.GetComponent<CanvasGroup>();
        _weaponHealth.alpha = 0;
        for (float count = 0; count < 1 || stopRoutine; count += Time.fixedDeltaTime) { _weaponHealth.alpha += Time.fixedDeltaTime; yield return new WaitForFixedUpdate(); }
        yield break;
    }

    public IEnumerator HideWeaponHealth()
    {
        weaponHealth.gameObject.SetActive(false);
        stopRoutine = true;
        yield return new WaitForEndOfFrame();
        stopRoutine = false;
        weaponHealth.gameObject.SetActive(true);


        CanvasGroup _weaponHealth = weaponHealth.GetComponent<CanvasGroup>();
        _weaponHealth.alpha = 1;
        for (float count = 0; count < 1 || stopRoutine; count += Time.fixedDeltaTime) { _weaponHealth.alpha -= Time.fixedDeltaTime; yield return new WaitForFixedUpdate(); }
        yield break;
    }
}
