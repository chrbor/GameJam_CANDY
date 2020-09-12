using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class CaddyContent : MonoBehaviour
{
    public float endSize = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FallIntoCaddy());
    }

    IEnumerator FallIntoCaddy()
    {
        float diff = transform.localScale.x - endSize;
        gameObject.SetActive(true);

        transform.localPosition = new Vector3(Random.Range(-0.1f, 0.3f), 1 + Random.Range(0f, 0.4f));
        float rotation = Random.Range(-15, 15);

        for (float count = 0; count < 1f; count += Time.fixedDeltaTime)
        {
            transform.Rotate(Vector3.forward, rotation);
            transform.localPosition -= Vector3.up * Time.fixedDeltaTime;
            transform.localScale -= Vector3.one * Time.fixedDeltaTime * diff;
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
