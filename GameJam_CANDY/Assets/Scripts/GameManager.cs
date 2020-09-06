using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    /// <summary> Mask für den Raycast, um Bodenkontakt festzustellen </summary>
    public static int rMask = (1 << 10) | (1 << 15);//Kontakt mit Ground und Einkaufswagen
    /// <summary> Das Ziel aller Süßwaren </summary>
    public GameObject caddy;
    /// <summary> Der Spiel- Charakter </summary>
    public GameObject player;
    /// <summary> Wenn wahr, dann läuft das Spiel weiter, wenn falsch, dann stoppt das Spiel </summary>
    public static bool run = true;

    void Awake()
    {
        //Erstelle Singleton:
        if (manager) { manager.caddy = caddy; manager.player = player; Destroy(gameObject); }
        manager = this;

        SceneManager.sceneLoaded += OnLevelLoaded;

        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }


    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        caddy = GameObject.FindGameObjectWithTag("Caddy");
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
