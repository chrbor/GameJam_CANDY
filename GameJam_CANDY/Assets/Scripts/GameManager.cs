using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    void Awake()
    {
        //Erstelle Singleton:
        if (manager != null) Destroy(gameObject);
        manager = this;

        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
