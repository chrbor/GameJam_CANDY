using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject hitPanel;
    private Image image;
    private bool overlaying;

    private void Awake()
    {
        hitPanel = transform.Find("HitPanel").gameObject;
        if (!hitPanel) { Debug.Log("Panel not found"); return; }
        image = hitPanel.GetComponent<Image>();
    }

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

    public IEnumerator HitOverlay()
    {
        if (!hitPanel || overlaying) yield break;
        overlaying = true;
        image.color = Color.red - Color.black;

        Debug.Log("Play hitpanel");
        hitPanel.SetActive(true);
        for (float count = 0; count < 0.2f; count += Time.deltaTime)
        {
            image.color = new Color(1, 0, 0, count);
            yield return new WaitForEndOfFrame();
        }
        for (float count = 0; count < 0.2f; count += Time.deltaTime)
        {
            image.color -= new Color(1, 0, 0, 0.2f - count);
            yield return new WaitForEndOfFrame();
        }
        hitPanel.SetActive(false);

        overlaying = false;
        yield break;
    }
}
