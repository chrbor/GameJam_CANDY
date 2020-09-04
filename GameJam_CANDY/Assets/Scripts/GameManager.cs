using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    /// <summary> Mask für den Raycast, um Bodenkontakt festzustellen </summary>
    public static int rMask = (1 << 10) | (1 << 15);//Kontakt mit Ground und Einkaufswagen
    /// <summary> Das Ziel aller Süßwaren </summary>
    public GameObject caddy;
    /// <summary> Wenn wahr, dann läuft das Spiel weiter, wenn falsch, dann stoppt das Spiel </summary>
    public static bool run = true;

    void Awake()
    {


        //Erstelle Singleton:
        if (manager) { manager.caddy = caddy; Destroy(gameObject); }
        manager = this;

        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
