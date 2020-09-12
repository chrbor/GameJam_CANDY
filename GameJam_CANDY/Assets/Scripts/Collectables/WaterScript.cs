using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class WaterScript : Baguette
{
    public float slowdown;

    public override void Initialize()
    {
        manager.player.GetComponent<PlayerScript>().acceleration /= slowdown;
        manager.player.GetComponent<PlayerScript>().maxSpeed /= slowdown;
    }

    public override void GetDestroyed()
    {
        manager.player.GetComponent<PlayerScript>().acceleration *= slowdown;
        manager.player.GetComponent<PlayerScript>().maxSpeed *= slowdown;
        base.GetDestroyed();
    }

    public override void Drop()
    {
        manager.player.GetComponent<PlayerScript>().acceleration *= slowdown;
        manager.player.GetComponent<PlayerScript>().maxSpeed *= slowdown;
        base.Drop();
    }

    public override IEnumerator Eat()
    {
        manager.player.GetComponent<PlayerScript>().acceleration *= slowdown;
        manager.player.GetComponent<PlayerScript>().maxSpeed *= slowdown;
        return base.Eat();
    }
}
