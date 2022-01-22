using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviour
{
    Patrol,
    HuntPlayer,
    Idle
}

public class NPC : MonoBehaviour
{
    public Behaviour behaviour;
    public GameManager gameManager;
    public PlayerControls playerControls;
    [HideInInspector]
    public int id;

    private void Start()
    {
        behaviour = Behaviour.Patrol;
    }

    public void Act()
    {
        if (playerControls.isDead)
        {
            EndTurn();
            return;
        }
        if (behaviour == Behaviour.Idle)
        {
            
        }
        else if (behaviour == Behaviour.Patrol)
        {
            StartCoroutine(Patrol());
            return;
        }
        else
        {

        }
        StartCoroutine(EndTurnAfterSeconds());
    }

    /// <summary>
    /// Walk around with no purpose
    /// </summary>
    private IEnumerator Patrol()
    {
        yield return new WaitForSeconds(1f);
        gameManager.DisplayActionRange(ActionType.Walk, playerControls.type);
        while (playerControls.tilesWalked < playerControls.playerSpeed)
        {
            playerControls.RandomMoveNPC();
            yield return new WaitForSeconds(GameData.current.npcMoveDuration);
        }
        EndTurn();
    }

    private IEnumerator EndTurnAfterSeconds()
    {
        Debug.Log("beginning wait");
        yield return new WaitForSeconds(1f);
        Debug.Log("end wait");
        EndTurn();
    }

    private void EndTurn()
    {
        gameManager.ProcessEndCharMove(playerControls.type, id);
    }
}
