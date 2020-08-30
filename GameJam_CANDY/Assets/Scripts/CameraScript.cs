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

    //Hilfsfunktionen:
    /// <summary>
    /// Wandelt die angegebene Gradzahl in einen Einheitsvektor, der in die Gradrichtung zeigt, um
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Vector2 RotToVec(float rotation)
    {
        return new Vector2(Mathf.Cos(rotation * Mathf.Deg2Rad), Mathf.Sin(rotation * Mathf.Deg2Rad));
    }
}
