using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    /// <summary> Liste an Wegpunkten wie Rolltreppen, Kanten oder Fahrstühlen, an denen sich die Gegner orientieren </summary>
    public static List<Vector2> Waypoints;

    void Start()
    {
        Waypoints.Insert(0, transform.position);
    }
}
