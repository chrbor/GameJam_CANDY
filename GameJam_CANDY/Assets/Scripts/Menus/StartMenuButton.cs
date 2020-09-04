using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Animator>().Play("Hover_Button", 0, (transform.parent.GetSiblingIndex() % 3) * 0.3f);
    }
}
