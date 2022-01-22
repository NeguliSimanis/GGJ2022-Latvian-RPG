using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static GameData current;

    #region GENERAL
    public int currentTurn = 1;
    public TurnType turnType = TurnType.Player;
    public float tileSize = 1f;
    #endregion

    #region NPC
    public float npcMoveDuration = 0.5f;
    public float npcActionDuration = 0.5f;
    #endregion

    #region XP BALANCE
    public int defaultLevelExp = 100; // how much xp is required to reach LV 2
    public float expIncrease = 1.1f;  // how fast xp requirement grows
    #endregion
}
