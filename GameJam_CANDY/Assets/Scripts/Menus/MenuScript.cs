using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    private GameObject hitPanel;
    private GameObject helpPanel;
    private Image image;
    private bool overlaying;

    public void Awake()
    {
        if(transform.Find("Help")) helpPanel = transform.Find("Help").gameObject;
        if (transform.Find("HitPanel"))
        {
            hitPanel = transform.Find("HitPanel").gameObject;
            image = hitPanel.GetComponent<Image>();
        }
        else Debug.Log("Panel not found");
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

    public void ShowHelp()
    {
        if(!helpPanel) { Debug.Log("There is no help >:D"); return; }
        StartCoroutine(ShowingHelp());
    }

    IEnumerator ShowingHelp()
    {
        Image img = helpPanel.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
        run = false;
        helpPanel.SetActive(true);
        for(float count = 0; count < 1f; count += Time.deltaTime) { img.color += Color.black * Time.deltaTime; yield return new WaitForEndOfFrame(); }
        yield return new WaitUntil(() => !Input.anyKey);
        yield return new WaitUntil(() => Input.anyKey);
        for (float count = 0; count < 1f; count += Time.deltaTime) { img.color += Color.black * Time.deltaTime; yield return new WaitForEndOfFrame(); }
        helpPanel.SetActive(false);
        run = true;
        yield break;
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
