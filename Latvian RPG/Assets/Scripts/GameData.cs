using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static GameData current;

    #region TESTING
    public bool isTestingMode = false; // turned on for testing game
    #endregion

    #region GENERAL
    public bool gameStarted = false;
    public int currentTurn = 1;
    public TurnType turnType = TurnType.Player;
    public float tileSize = 1f;
    public int charactersSortingOrder = 3;
    #endregion

    #region TURNDURATION
    public float npcMoveDuration = 0.5f;
    public float npcActionDuration = 0.9f;
    public float playerMoveDuration = 0.35f;

    public float playerTurnTimer = 15f;
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
    public int pointsRequiredPhase2 = 35;
    public int pointsRequiredPhase3 = 100;

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
}
