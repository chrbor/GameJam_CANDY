using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;
using static GameManager;

public class GameMenu : MenuScript
{
    public static GameMenu gameMenu;

    private GameObject hud;
    private Slider healthbar;
    private Text healthInfo;
    private Slider caddySlider;
    private Text caddyInfo;
    private GameObject weaponHealth;
    private Slider weaponSlider;
    private Text weaponInfo;
    private Image weaponImage;

    private bool stopRoutine;

    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();

        gameMenu = this;

        hud = transform.Find("HUD").gameObject;
        healthbar = hud.transform.Find("Health").GetComponent<Slider>();
        healthInfo = healthbar.transform.Find("Info").GetComponent<Text>();

        caddySlider = hud.transform.Find("Caddy").GetComponent<Slider>();
        caddyInfo = caddySlider.transform.Find("Info").GetComponent<Text>();

        weaponHealth = hud.transform.Find("Weapon").gameObject;
        weaponInfo = weaponHealth.transform.Find("Info").GetComponent<Text>();
        weaponSlider = weaponHealth.GetComponent<Slider>();
        weaponImage = weaponHealth.transform.Find("Image").GetComponent<Image>();

        weaponHealth.SetActive(false);
    }

    /// <summary>
    /// Aktualisiert die Lebensleiste
    /// </summary>
    /// <param name="healthbar_left"></param>
    public void SetHealth(int healthbar_left)
    {
        healthbar.value = healthbar_left;
        healthInfo.text = healthbar_left.ToString() + "/100"; 
        if (healthbar_left <= 0) StartCoroutine(HideWeaponHealth());

        //Hier der Code um die Leiste langsam zu dem neuen Wert übergehen zu lassen P:
    }
    
    /// <summary>
    /// Aktualisiert die Einkaufsleiste
    /// </summary>
    /// <param name="healthbar_left"></param>
    public void SetCaddyHealth(int caddyHealth_left)
    {
        caddySlider.value = caddyHealth_left;
        caddyInfo.text = caddyHealth_left.ToString() + "/" + gameController.candyLimit; 
        if (caddyHealth_left <= 0) StartCoroutine(HideWeaponHealth());

        //Hier der Code um die Leiste langsam zu dem neuen Wert übergehen zu lassen P:
    }

    public void SetNewCaddy()
    {
        caddySlider.maxValue = gameController.candyLimit;
        gameController.candyCount = gameController.candyLimit;
        SetCaddyHealth(gameController.candyLimit);
    }

    /// <summary>
    /// Ändert die Anzeige zur neuen Waffe
    /// </summary>
    /// <param name="sprite"></param>
    public void SetNewWeapon(GameObject weapon)
    {
        weaponImage.sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        weaponHealth.SetActive(true);
        weaponSlider.maxValue = weapon.GetComponent<CollBase>().weapon.maxHealth;
        SetWeaponHealth(weapon.GetComponent<CollBase>().weapon.health);
    }

    public void SetWeaponHealth(int health)
    {
        weaponSlider.value = health;
        weaponInfo.text = health.ToString() + "/" + weaponSlider.maxValue.ToString();
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


    public void GameOver()
    {
        run = false;
        Transform menu = transform.GetChild(3);
        menu.Find("GameOver").gameObject.SetActive(true);
        menu.Find("LevelComplete").gameObject.SetActive(false);
        Transform buttons = menu.GetChild(0);
        buttons.Find("NextButton").gameObject.SetActive(false);

        Canvas.ForceUpdateCanvases();
        buttons.GetComponent<HorizontalLayoutGroup>().enabled = false;
        buttons.GetComponent<HorizontalLayoutGroup>().enabled = true;

        menu.gameObject.SetActive(true);
    }

    public void LevelComplete()
    {
        run = false;
        Transform menu = transform.GetChild(3);
        menu.Find("GameOver").gameObject.SetActive(false);
        menu.Find("LevelComplete").gameObject.SetActive(true);
        Transform buttons = menu.GetChild(0);
        buttons.Find("NextButton").gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();
        buttons.GetComponent<HorizontalLayoutGroup>().enabled = false;
        buttons.GetComponent<HorizontalLayoutGroup>().enabled = true;

        menu.gameObject.SetActive(true);
    }
}
