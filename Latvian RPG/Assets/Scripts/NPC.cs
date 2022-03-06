﻿using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
public enum Behaviour
{
    Patrol,
    HuntPlayer,
    Idle,
    Flee
}

public class NPC : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer npcSpriteRenderer;
    private int defaultSortingOrder;

    private Skill selectedSkill;
    private PlayerControls closestPlayer;

    private float delayBeforeActionStart = 0.9f;

    public Behaviour behaviour;
    public GameManager gameManager;
    public PlayerControls npcControls;
    [HideInInspector]
    public int id;

    private Dictionary<Direction, bool> occuppiedDirections = new Dictionary<Direction, bool>();

    private void Start()
    {
        behaviour = Behaviour.Patrol;
        defaultSortingOrder = npcSpriteRenderer.sortingOrder;
        occuppiedDirections.Add(Direction.Down, false);
        occuppiedDirections.Add(Direction.Right, false);
        occuppiedDirections.Add(Direction.Up, false);
        occuppiedDirections.Add(Direction.Left, false);

    }

    public void Act()
    {
        if (npcControls.isDead)
        {
            EndTurn();
            return;
        }
        // CheckForNearbyObstructions();
        gameManager.HighlightChar(npcControls, highlight: true);

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
            case Behaviour.Flee:
                Flee();
                return;
        }
    }

    private void CheckForNearbyObstructions()
    {
        occuppiedDirections[Direction.Right] = false;
        occuppiedDirections[Direction.Left] = false;
        occuppiedDirections[Direction.Up] = false;
        occuppiedDirections[Direction.Down] = false;
        foreach (Obstacle obstacle in gameManager.allObstacles)
        {
            if (obstacle.pos.y == npcControls.yCoord)
            {
                // right
                if (obstacle.pos.x + 1 == npcControls.xCoord)
                {
                    occuppiedDirections[Direction.Right] = true;
                }
                // left
                else if (obstacle.pos.x - 1 == npcControls.xCoord)
                {
                    occuppiedDirections[Direction.Left] = true;
                }
            }
            else if (obstacle.pos.x == npcControls.xCoord)
            {
                // up
                if (obstacle.pos.y + 1 == npcControls.xCoord)
                {
                    occuppiedDirections[Direction.Up] = true;
                }
                // down
                else if (obstacle.pos.y - 1 == npcControls.xCoord)
                {
                    occuppiedDirections[Direction.Down] = true;
                }
            }
        }
        foreach (KeyValuePair<Direction, bool> curDirection in occuppiedDirections)
        {
            if (curDirection.Value == true)
            {
                Debug.Log(npcControls.name + " is occuppued to  " + curDirection.Key);
            }
        }
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
        DisplayNPCWalkRange();
        yield return new WaitForSeconds(delayBeforeActionStart);
        int whileCounter = 10;
        while (npcControls.tilesWalked < npcControls.playerSpeed && whileCounter > 0)
        {
            if (npcControls.RandomMoveNPC())
                yield return new WaitForSeconds(GameData.current.npcMoveDuration);
            whileCounter--;
        }
        EndTurn();
    }

    private IEnumerator HuntPlayer(bool suicideHunt = false)
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
            if (!suicideHunt)
            {
                SetBehaviour(Behaviour.Flee);
                Flee();
            }
            else
            {
                Debug.Log(npcControls.name + "should END TURN DUDE");
                EndTurn();
            }
            yield break;
        }
        Debug.Log("BREAK 2 didnt WORK - damaging skill selected");

        if (!IsTargetInSkillRange(target:closestPlayer, skill: selectedSkill))
        {
            Debug.Log("TARGET NOT IN SKILL RANGE");
            DisplayNPCWalkRange();
            if (npcControls.tilesWalked < npcControls.stats.speed)
                StartCoroutine(MoveCloserToTarget(
                    target: new Vector2Int(closestPlayer.xCoord, closestPlayer.yCoord),
                    endTurnAfter:false,
                    attemptAttackAfter: true));
            yield break;
        }
        Debug.Log("BREAK 3 didnt WORK - target within damaging skill range");
        StartCoroutine(UseDamageSkillOnTarget(target: closestPlayer, skill: selectedSkill));

        yield break;
    }

    private void DisplayNPCWalkRange([CallerMemberName] string callerName = "")
    {
        Debug.Log("DisplayNPCWalkRange called by " + callerName);
        npcSpriteRenderer.sortingOrder = defaultSortingOrder + 7;
        gameManager.DisplayActionRange(ActionType.Walk, npcControls.type);
    }

    private bool IsMyTurn()
    {
        if (GameData.current.turnType == TurnType.Enemy && npcControls.type == CharType.Enemy)
        {
            return true;
        }
        if (GameData.current.turnType == TurnType.Neutral && npcControls.type == CharType.Neutral)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator MoveCloserToTarget(Vector2Int target, bool endTurnAfter = false, bool attemptAttackAfter = false, [CallerMemberName] string callerName = "")
    {
        Debug.Log(npcControls.name + "MOVING CLOSER TO TARGET. Method called by " + callerName);
        while (npcControls.tilesWalked < npcControls.playerSpeed)// && IsMyTurn())
        {
            yield return new WaitForSeconds(delayBeforeActionStart);
            // FURTHER ON X AXIS - move closer on X axis
            if (Mathf.Abs(target.x - npcControls.xCoord) >
                Mathf.Abs(target.y - npcControls.yCoord))
            {
                if (target.x > npcControls.xCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Right);
                }
                else if (target.x < npcControls.xCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Left);
                }
            }
            // FURTHER ON Y AXIS - move closer on Y axis
            else
            {
                if (target.y > npcControls.yCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Up);
                }
                else if (target.y < npcControls.yCoord)
                {
                    npcControls.MoveCharacterOneTile(Direction.Down);
                }
            }
            Debug.Log(npcControls.name + " MOVED TO " + npcControls.xCoord + "." + npcControls.yCoord + ". CURR TILES WALKED + " + npcControls.tilesWalked);
            npcControls.tilesWalked++;
        }
        if (attemptAttackAfter)
        {
            StartCoroutine(AttackIfPossible(target: closestPlayer));
        }
        if (endTurnAfter)
        {
            yield return new WaitForSeconds(GameData.current.npcMoveDuration*1.1f);
            EndTurn();
        }
    }

    /// <summary>
    /// WARNING: WON'T WORK CORRECTLY IF HASN'T MOVED NEXT TO TARGET YET
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator AttackIfPossible(PlayerControls target)
    { 
            // CHECK IF IN SKILL RANGE NOW
            if (IsTargetInSkillRange(target: target, skill: selectedSkill))
            {
                StartCoroutine(UseDamageSkillOnTarget(target, selectedSkill));
                yield break;
            }
            yield return new WaitForSeconds(GameData.current.npcMoveDuration);
        
        EndTurn();
        yield break;
    }


    private void Flee()
    {
        Debug.Log("FLEEING ENEMY COORD = " + npcControls.xCoord + "." + npcControls.yCoord);
        DisplayNPCWalkRange();

        int xMin = npcControls.xCoord - npcControls.playerSpeed;
        int xMax = npcControls.xCoord + npcControls.playerSpeed;
        int yMin = npcControls.yCoord - npcControls.playerSpeed;
        int yMax = npcControls.yCoord + npcControls.playerSpeed;

        Vector2Int npcCoord = new Vector2Int(npcControls.xCoord, npcControls.yCoord);
        Vector2Int defaultSafestCoord = new Vector2Int(99999, 99999);
        Vector2Int safestCoord = new Vector2Int(xMin, yMin);
        Vector2Int coordToCheck = safestCoord;

        bool safeSpaceFound = true;

        // 1. Go through all players chars
        foreach (PlayerControls player in gameManager.allCharacters)
        {
            if (player.type == CharType.Player)
            {
                // 2. Current safe space not safe after all - find a new one
                if (!IsCoordinateSafeFromPlayer(player, safestCoord)
                    || !gameManager.IsTileAllowedForNPC(safestCoord))
                {
                    safeSpaceFound = false;

                    // 2.1. go through all remaining coords
                    for (int xCoord = safestCoord.x; xCoord < xMax; xCoord++)
                    {
                        if (safeSpaceFound)
                            break;
                        for (int yCoord = safestCoord.y; yCoord < yMax; yCoord++)
                        {
                            coordToCheck = new Vector2Int(xCoord, yCoord);
                            // 2.2 SAFE SPACE FOUND!
                            if (IsCoordinateSafeFromPlayer(player, coordToCheck)
                                && gameManager.IsTileAllowedForNPC(coordToCheck))
                            {
                                safestCoord = coordToCheck;
                                safeSpaceFound = true;
                            }
                        }
                    }
                }
            }
        }

        // 4. Go to safe coord
        if (safeSpaceFound)
        {
            Debug.Log(npcControls.name + "MOVING TO SAFEST COORD:    " + safestCoord);
            StartCoroutine(MoveCloserToTarget(target: safestCoord, endTurnAfter: true));
        }
        else
        {
            // 5. No safe place exists - skip movement 
            Debug.Log("NO SAFE SPACE FOUND!!!");
            // 6. No safe place exists & enough mana - suicide attack
           StartCoroutine(HuntPlayer(suicideHunt:true));
        }
    }

    private bool IsCoordinateSafeFromPlayer(PlayerControls player, Vector2Int coord)
    {

        if (gameManager.IsTileOccupiedByObstacle(coord))
        {
            Debug.Log(coord + "obstacle on tile, cannot move there");
            return false;
        }

        bool isInDanger = false;
        isInDanger = MathUtils.IsWithinDamageRange(
            target: coord,                                                  // npc
            damageSource: new Vector2Int(player.xCoord, player.yCoord),    // player
            damageSkill: player.GetLongestRangeDamageSkill(),               // 
            moveSpeed: player.playerSpeed);

        return !isInDanger;
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
                        gameManager.selectedSkill = skill;
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
            if (player.type == CharType.Player && !player.isDead)
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
        if (anyPlayerFound)
        {
            Debug.Log(npcControls.name + " found the closest character - " + closestPlayer.name + " at " + closestPlayer.xCoord + "." + closestPlayer.yCoord);
        }
        return anyPlayerFound;
    }

    private bool IsTargetInSkillRange(PlayerControls target, Skill skill)
    {
        #region
        /*
         *      x = target, coordinate 2.2
         *      0 = source, coordinate 0.0
         *      1 = tiles within 2 tile range
         *      
         *      
         *      _ _ 1 _ x 
         *      _ 1 1 1 _
         *      1 1 0 1 1  
         *      _ 1 1 1 _  
         *      _ _ 1 _ _  
         *
         * 
         *      xDiff = abs(2-0) = 2
         *      xDiff = abs(2-0) = 2
         *      sum = 4
         *      
         *      yup seems correct
         */
        #endregion
        int xDiff = Mathf.Abs(target.xCoord - npcControls.xCoord);
        int yDiff = Mathf.Abs(target.yCoord - npcControls.yCoord);
        if (xDiff + yDiff <= skill.skillRange)
            return true;
        return false;
    }

    private bool IsThisPlayerCloser(PlayerControls thisPlayer)
    {
        Debug.Log("checking " + thisPlayer.name);
        int diffX = Mathf.Abs(npcControls.xCoord - closestPlayer.xCoord);
        int diffY = Mathf.Abs(npcControls.yCoord - closestPlayer.yCoord);

        int newDiffX = Mathf.Abs(npcControls.xCoord - thisPlayer.xCoord);
        int newDiffY = Mathf.Abs(npcControls.yCoord - thisPlayer.yCoord);

        Debug.Log("NEW DIFFX" + newDiffX + ". NEW DIFFY " + newDiffY);
        Debug.Log("old DIFFX" + diffX + ". old DIFFY " + diffY);

        if (newDiffX + newDiffY < diffX + diffY)
        {
            Debug.Log(npcControls.name + "(" + npcControls.xCoord + "." + npcControls.yCoord + " is closer to " +
                thisPlayer.name + "(" + thisPlayer.xCoord + "." + thisPlayer.yCoord + " than " +
                closestPlayer.name + "(" + closestPlayer.xCoord + "." + closestPlayer.yCoord);
            return true;
        }

        return false;
    }

    private IEnumerator UseDamageSkillOnTarget(PlayerControls target, Skill skill)
    {
        yield return new WaitForSeconds(GameData.current.npcActionDuration * 1);
        gameManager.HideActionRange();
        gameManager.DisplayActionRange(ActionType.UseCombatSkill, CharType.Enemy);
        yield return new WaitForSeconds(GameData.current.npcActionDuration * 3);
        target.TakeDamage(amount: -skill.skillDamage, damageSource: npcControls);
        npcControls.SpendMana(skill.manaCost);

        // DECIDE ON WHAT TO DO AFTER ATTACKING
        

        // flee if dont have mana
        bool allCharsDead = true;
        foreach (PlayerControls character in gameManager.allCharacters)
        {
            if (!character.isDead)
            {
                allCharsDead = false;
                if (npcControls.stats.currMana < skill.manaCost)
                {
                    gameManager.HideActionRange();
                    SetBehaviour(Behaviour.Flee);
                    Flee();
                    yield break;
                }
            }
        }

        
        if (npcControls.stats.currMana >= skill.manaCost)
        {
            // stay in place if alive and enough mana to attack again
            if (!target.isDead)
            {
                Debug.Log("should stay in place to attack again");
                EndTurn();
                yield break;
            }
            else
            {
                Debug.Log("OTHER TARGETS AVAILABLE, SHOULD IMPLEMENT NEW HUNT");
            }
        }


        Debug.Log(npcControls.name + " IS HERE");
        EndTurn();

        // end turn if all characters dead
        if (allCharsDead)
        {
            Debug.Log("All chars dead");
            
        }

    }


    private IEnumerator EndTurnAfterSeconds(float seconds)
    {
        Debug.Log("beginning wait");
        yield return new WaitForSeconds(seconds);
        Debug.Log("end wait");
        EndTurn();
    }

    private void EndTurn([CallerMemberName] string callerName = "")
    {
        Debug.Log("end turn called by " + callerName);
        npcSpriteRenderer.sortingOrder = defaultSortingOrder;
        gameManager.HighlightChar(npcControls, highlight: false);
        gameManager.ProcessEndCharMove(npcControls.type, id);
    }
}
