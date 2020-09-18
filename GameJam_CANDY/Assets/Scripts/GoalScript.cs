using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static GameMenu;
using static GameController;

public class GoalScript : MonoBehaviour
{
    //Wahr, wenn der Einkaufswagen bei der kasse ist
    private bool caddyIn;
    private Animator anim;


    void Start()
    {
        anim = transform.GetChild(1).GetComponent<Animator>();

        caddyIn = gameController.withoutCaddy;
    }

    public bool EndLevel()
    {
        if(!caddyIn) { Debug.Log("Can't finish without caddy!"); return false; }

        bool finished = gameController.CheckIfFinished();
        if (!finished) { Debug.Log("Havn't finished yet"); return false; }
        Debug.Log("You finished the level!");
        gameMenu.LevelComplete();

        return finished;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Caddy") && !gameController.withoutCaddy) caddyIn = true;
        else if (other.CompareTag("Player")) anim.SetTrigger("wave");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Caddy") && !gameController.withoutCaddy) caddyIn = false;
        else if (other.CompareTag("Player")) anim.SetTrigger("leave");
    }
}
