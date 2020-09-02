using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator PulsingLight()
    {
        while (true)
        {

            yield return new WaitForFixedUpdate();
        }
    }
}
