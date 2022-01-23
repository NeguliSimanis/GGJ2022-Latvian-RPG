using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum Behaviour
{
    Patrol,
    HuntPlayer,
    Idle,
    Flee
}

public class NPC : MonoBehaviour
{
    bool hasCombatSkills = false; // skills that deal direct damage
    bool hasCombatUtilitySkills = false; // skills that don't deal damage but cause debuffs
    bool hasSupportSkills = false; // skills that help other chars and dont deal damage

    Skill selectedSkill;
    PlayerControls closestPlayer;

    float delayBeforeActionStart = 0.9f;

    public Behaviour behaviour;
    public GameManager gameManager;
    public PlayerControls npcControls;
    [HideInInspector]
    public int id;

    private void Start()
    {
        behaviour = Behaviour.Patrol;
    }

    public void Act()
    {
        if (npcControls.isDead)
        {
            EndTurn();
            return;
        }

        SetBehaviour();

        switch(behaviour)
        {
            case Behaviour.Idle:
                break;
            case Behaviour.Patrol:
                StartCoroutine(Patrol());
                return;
            case Behaviour.HuntPlayer:
                StartCoroutine(HuntPlayer());
                return;
        }
        StartCoroutine(EndTurnAfterSeconds());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newBehaviour">if this is default, then will choose behaviour based on character type (enemy/neutral)</param>
    private void SetBehaviour(Behaviour newBehaviour = Behaviour.Idle)
    {

        if (npcControls.type == CharType.Neutral || newBehaviour == Behaviour.Patrol)
        {
            behaviour = Behaviour.Patrol;
        }
        else if (newBehaviour == Behaviour.Flee)
        {
            Debug.Log(npcControls.name + " SHOULD FLEE");
            behaviour = Behaviour.Flee;
        }
        else if (npcControls.type == CharType.Enemy || newBehaviour == Behaviour.HuntPlayer)
        {
            Debug.Log(npcControls.name + " SHOULD HUNT");
            behaviour = Behaviour.HuntPlayer;
        }
        
    }

    /// <summary>
    /// Walk around with no purpose
    /// </summary>
    private IEnumerator Patrol()
    {
        yield return new WaitForSeconds(delayBeforeActionStart);
        gameManager.DisplayActionRange(ActionType.Walk, npcControls.type);
        while (npcControls.tilesWalked < npcControls.playerSpeed)
        {
            npcControls.RandomMoveNPC();
            yield return new WaitForSeconds(GameData.current.npcMoveDuration);
        }
        EndTurn();
    }

    private IEnumerator HuntPlayer()
    {
        Debug.Log("hunt started");
        yield return new WaitForSeconds(delayBeforeActionStart);

        if (!FindClosestPlayer())
        {
            SetBehaviour(Behaviour.Patrol);
            StartCoroutine(Patrol());
            yield break;
        }
        Debug.Log("BREAK 1 didnt WORK - closest player found");

        if (!SelectDamagingSkill())
        {
            SetBehaviour(Behaviour.Flee);
            StartCoroutine(Flee());
            yield break;
        }
        Debug.Log("BREAK 2 didnt WORK - damaging skill selected");

        if (!IsTargetInSkillRange(target:closestPlayer, skill: selectedSkill))
        {
            StartCoroutine(MoveCloserToTarget(target: closestPlayer));
            yield break;
        }
        Debug.Log("BREAK 3 didnt WORK - target within damaging skill range");
        StartCoroutine(UseDamageSkillOnTarget(target: closestPlayer, skill: selectedSkill));

        yield break;
    }

    private IEnumerator MoveCloserToTarget(PlayerControls target)
    {
        Debug.Log("MOVING CLOSER TO TARGET");
        yield return new WaitForSeconds(delayBeforeActionStart);
        gameManager.DisplayActionRange(ActionType.Walk, npcControls.type);
        
        while (npcControls.tilesWalked < npcControls.playerSpeed)
        {
            // FURTHER ON X AXIS - move closer on X axis
            if (Mathf.Abs(target.xCoord - npcControls.xCoord) >
                Mathf.Abs(target.yCoord - npcControls.yCoord))
            {
                if (target.xCoord > npcControls.xCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Right);
                }
                else if (target.xCoord < npcControls.xCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Left);
                }
            }
            // FURTHER ON Y AXIS - move closer on Y axis
            else 
            {
                if (target.yCoord > npcControls.yCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Up);
                }
                else if (target.yCoord < npcControls.yCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Down);
                }
            }
            npcControls.tilesWalked++;
            
            // CHECK IF IN SKILL RANGE NOW
            if (IsTargetInSkillRange(target: target, skill: selectedSkill))
            {
                StartCoroutine(UseDamageSkillOnTarget(target, selectedSkill));
                yield break;
            }

            yield return new WaitForSeconds(GameData.current.npcMoveDuration);
        }
        EndTurn();
        yield break;
    }

    private IEnumerator Flee()
    {
        Debug.Log("FLEEING NOT IMPLEMENTED");
        yield return new WaitForSeconds(delayBeforeActionStart);
    }

    private bool SelectDamagingSkill()
    {
        foreach (Skill skill in npcControls.startingSkills)
        {
            foreach (SkillType type in skill.type)
            {
                if (type == SkillType.Damage)
                {
                    if (npcControls.stats.currMana >= skill.manaCost)
                    {
                        selectedSkill = skill;
                        return true;
                    }
                    else
                        Debug.Log("NOT ENOUGH MANA TO USE SKILL");
                }
            }
        }
        return false;
    }

    private bool FindClosestPlayer()
    {
        bool anyPlayerFound = false;
        foreach (PlayerControls player in gameManager.allCharacters)
        {
            if (player.type == CharType.Player)
            {
                if (!anyPlayerFound)
                {
                    closestPlayer = player;
                }
                anyPlayerFound = true;
                if (IsThisPlayerCloser(player))
                {
                    closestPlayer = player;
                }
            }
        }
        return anyPlayerFound;
    }

    private bool IsTargetInSkillRange(PlayerControls target, Skill skill)
    {
        int xDiff = Mathf.Abs(target.xCoord - npcControls.xCoord);
        int yDiff = Mathf.Abs(target.yCoord - npcControls.yCoord);
        if (xDiff + yDiff <= skill.skillRange)
            return true;
        return false;
    }

    private bool IsThisPlayerCloser(PlayerControls thisPlayer)
    {
        int diffX = Mathf.Abs(npcControls.xCoord - closestPlayer.xCoord);
        int diffY = Mathf.Abs(npcControls.yCoord - closestPlayer.yCoord);

        int newDiffX = Mathf.Abs(npcControls.xCoord - thisPlayer.xCoord);
        int newDiffY = Mathf.Abs(npcControls.xCoord - thisPlayer.xCoord);

        if (newDiffX + newDiffY < diffX + diffY)
            return true;

        return false;
    }

    private IEnumerator UseDamageSkillOnTarget(PlayerControls target, Skill skill)
    {
        target.TakeDamage(amount: -skill.skillDamage, damageSource: npcControls);
        npcControls.SpendMana(skill.manaCost);

        // DECIDE ON WHAT TO DO AFTER ATTACKING
        yield return new WaitForSeconds(GameData.current.npcActionDuration*3);

        // stay in place if alive and enough mana to attack again
        if (!target.isDead && npcControls.stats.currMana >= skill.manaCost)
        {
            Debug.Log("should stay in place to attack again");
            EndTurn();
        }
        // flee if don't have mana
        else
        {
            SetBehaviour(Behaviour.Flee);
            StartCoroutine(Flee());
        }

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
        gameManager.ProcessEndCharMove(npcControls.type, id);
    }
}
