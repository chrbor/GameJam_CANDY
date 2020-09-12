using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MenuScript
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForStart());
    }

    // Update is called once per frame
    IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(2);
        CanvasGroup txt = transform.Find("PressKey").GetComponent<CanvasGroup>();
        txt.alpha = 0;

        txt.gameObject.SetActive(true);
        for(float count = 0; count < 1f; count += Time.deltaTime) { txt.alpha += Time.deltaTime; yield return new WaitForEndOfFrame(); }
        yield return new WaitUntil(() => Input.anyKey);
        for (float count = 0; count < 1f; count += Time.deltaTime) { txt.alpha -= Time.deltaTime; yield return new WaitForEndOfFrame(); }
        txt.gameObject.SetActive(false);


        CanvasGroup menu = transform.Find("Menu").GetComponent<CanvasGroup>();
        menu.alpha = 0;
        transform.Find("Menu").gameObject.SetActive(true);
        for (float count = 0; count < 1f; count += Time.deltaTime) { menu.alpha += Time.deltaTime; yield return new WaitForEndOfFrame(); }

        yield break;
    }
}
