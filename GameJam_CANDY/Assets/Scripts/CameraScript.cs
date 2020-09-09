using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /// <summary> Das GameObject, das im Focus der Kamera ist </summary>
    public GameObject focus;
    /// <summary> wie stark die Camera gedämpft wird </summary>
    [Range(1f, 1000f)]
    public float damping = 100;

    /// <summary> Differenz der Positionen zwischen der Kamera und dem focus </summary>
    private Vector2 diff;

    private bool shaking;

    // Start is called before the first frame update
    void Start()
    {
        if (!focus) focus = GameObject.FindGameObjectWithTag("Player");
        if (!focus) { Debug.Log("Player not found"); Destroy(this); }
    }

    void FixedUpdate()
    {
        diff = focus.transform.position - transform.position;

        transform.position += (Vector3)(diff * diff * diff / (diff.SqrMagnitude() * damping));

    }

    public IEnumerator Shake()
    {
        if (shaking) yield break;
        shaking = true;
        float strength = Camera.main.orthographicSize * 0.02f;
        for(float count = 0; count < 0.4f; count += Time.deltaTime)
        {
            transform.position += (Vector3)Random.insideUnitCircle * strength;
            yield return new WaitForEndOfFrame();
        }
        shaking = false;
        yield break;
    }
}
