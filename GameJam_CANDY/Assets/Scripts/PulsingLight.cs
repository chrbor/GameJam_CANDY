using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PulsingLight : MonoBehaviour
{
    public float speed;
    public float min, max;

    Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        light.intensity = min + (Mathf.Sin(Time.fixedTime * speed) + 1) * 0.5f * (max - min);
    }
}
