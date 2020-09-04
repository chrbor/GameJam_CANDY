using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class MenuScript : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        run = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
