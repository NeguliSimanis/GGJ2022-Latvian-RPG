using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class GameData
{
    public static GameData current;

    #region TESTING
    public bool isDebugMode = true; // turned on for testing game
    #endregion

    #region GENERAL
    private bool gamePaused = false;
    public bool gameStarted = false;
    public int currentTurn = 1;
    public CharType turnType = CharType.Player;
    public float tileSize = 1f;
    public int charactersSortingOrder = 3;
    #endregion

    #region DUNGEON
    public int dungeonFloor = -1;
    public static int totalFloorsCleared = 0;
    public static int maxFloorReached = 0;
    public bool secondSkillFloorSpawned = false;
    public int floorsUntilScholar = 4;
    #endregion

    #region TURNDURATION
    public bool hasTurnDuration = false;

    public float npcMoveDuration = 0.5f;
    public float npcActionDuration = 0.9f;
    public float playerMoveDuration = 0.35f;

    public float playerTurnTimer = 17f;
    public float playerTurnIncrease = 4f;
    public float playerTurnEndTime;
    public float playerTurnStartTime;
    #endregion

    #region XP BALANCE
    public int defaultLevelExp = 10; // how much xp is required to reach LV 2
    public float expIncrease = 1.1f;  // how fast xp requirement grows
    #endregion

    #region MOON BALANCE
    public int currMoonPoints = 0;
    public int pointsRequiredPhase1 = 1;
    public int pointsRequiredPhase2 = 55;
    public int pointsRequiredPhase25 = 120;
    public int pointsRequiredPhase3 = 190;

    public int killPointsReward = 5;
    public int levelUpPointsReward = 30;
    public int healPointsReward = 5;
    public int recruitPointsReward = 10;


    /*
     * Kill - 10 dark points
     * Level up choice - 20 dark/light points
     * Recruit - 20 light points 
     * Heal - 5 light points
     * Victory - 100 light/dark points
     */
    #endregion

    public void EnterNextFloor()
    {
        dungeonFloor++;
        totalFloorsCleared++;
        ResetTurnTimer();
        //maxFloorReached = dungeonFloor + 2;
        //dungeonFloor += 4;
        //totalFloorsCleared += 4;
    }

    private void ResetTurnTimer()
    {
        playerTurnStartTime = Time.time;
        playerTurnEndTime = playerTurnStartTime + playerTurnTimer;
    }

    public void UpdateMaxFloorReached()
    {
        maxFloorReached = RealFloor();
    }

    public int RealFloor()
    {
        return dungeonFloor + 2;
    }

    public bool isGamePaused
    {
        get
        {
            return gamePaused;
        }
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            gamePaused = true;
            Time.timeScale = 0f;
        }
        else
        {
            gamePaused = false;
            Time.timeScale = 1f;
        }
    }
}
