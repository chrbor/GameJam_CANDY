using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    /// <summary> Mask für den Raycast, um Bodenkontakt festzustellen </summary>
    public static int rMask = (1 << 10) | (1 << 15) | (1 << 21);//Kontakt mit Ground, Einkaufswagen und Slant
    /// <summary> Das Ziel aller Süßwaren </summary>
    public GameObject caddy;
    /// <summary> Der Spiel- Charakter, existiert in jedem Level genau einmal </summary>
    public GameObject player;
    /// <summary> Das aktuelle MenuScript </summary>
    [HideInInspector]
    public MenuScript menu;
    /// <summary> Wenn wahr, dann läuft das Spiel weiter, wenn falsch, dann stoppt das Spiel </summary>
    public static bool run = true;

    //Vorladen der Prefabs:
    /// <summary> Gameobjekt, das für den Rest des Spiels im Wagen verbleibt </summary>
    [HideInInspector]
    public GameObject caddyContent;
    /// <summary> Gameobjekt, die Eigenschaften einer Aufgabe im HUD anzeigt </summary>
    [HideInInspector]
    public GameObject taskField;

    void Awake()
    {
        //Erstelle Singleton:
        if (manager) Destroy(gameObject);
        manager = this;

        SceneManager.sceneLoaded += OnLevelLoaded;
        DontDestroyOnLoad(gameObject);

        //Vorladen:
        caddyContent = Resources.Load<GameObject>("Prefabs/CaddyContent");
        taskField = Resources.Load<GameObject>("Prefabs/Task");
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    IEnumerator RunGame()
    {
        //Hier den Code für Startsequenzen einsetzen

        run = true;
        while (player)//solange der Spieler existiert, existiert ein Level
        {
            if (run)
            {
                if (Input.GetKey(KeyCode.M))
                {
                    run = false;
                    menu.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                }
            }

            Physics2D.autoSimulation = run;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        caddy = GameObject.FindGameObjectWithTag("Caddy");
        player = GameObject.FindGameObjectWithTag("Player");
        menu = GameObject.Find("Canvas").GetComponent<MenuScript>();

        if (player) StartCoroutine(RunGame());
    }
}
