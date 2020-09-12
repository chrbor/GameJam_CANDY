using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        RectTransform rect = GetComponent<RectTransform>();
        yield return new WaitUntil(()=>!Input.anyKey);

        while(rect.anchoredPosition.y < 0)
        {
            if (Input.GetKey(KeyCode.Escape)) break;

            transform.position += Vector3.up * Time.fixedDeltaTime * speed * (Input.anyKey? 10:1);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => Input.anyKey);
        Debug.Log("Loading Menu");
        SceneManager.LoadScene(0);
        yield break;
    }
}
