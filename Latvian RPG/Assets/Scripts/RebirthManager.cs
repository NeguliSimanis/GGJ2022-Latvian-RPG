using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthBonus
{
    public CharStat type;
    public int amount;
    public int hiddenAmount = 0;
    public bool alreadyDisplayed = false;
    public bool appliedToChar = false;
    public bool chosen = false; // bonus chosen at the rebirth screen, needs to be applied

    public RebirthBonus(CharStat type, int newAmount)
    {
        this.type = type;
        amount = newAmount;
    }
}

public class RebirthManager : MonoBehaviour
{
    public static RebirthManager instance;
    public List<RebirthBonus> rebirthBonuses = new List<RebirthBonus>();

    public RebirthManager()
    {
        foreach (CharStat newStat in System.Enum.GetValues(typeof(CharStat)))
        {
            RebirthBonus newBonus = new RebirthBonus(newStat, 0);
            rebirthBonuses.Add(newBonus);
        }
    }

    public bool HasRebirthBonus()
    {
        bool hasRebirthBonus = false;
        foreach (RebirthBonus rebirthBonus in rebirthBonuses)
        {
            if (rebirthBonus.amount > 0)
                return true;
        }
        return hasRebirthBonus;
    }


    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    /// <summary>
    /// 
    /// Rolls what rebirth bonuses will be available.
    /// The bonuses depend on how many floors have been cleared:
    /// 1,2 = XP BONUS
    /// 3 = XP BONUS OR MANA
    /// 4, 5,6 = OFFENSE OR MANA
    /// 7, 9 = offfense/defense/mana
    /// 10+ = offense/defense/mana/life
    /// 
    /// </summary>
    /// <returns>
    /// 
    /// 
    /// 
    /// </returns>
    public void RollRebirthBonus()
    {

        foreach (RebirthBonus rebirthBonus in rebirthBonuses)
        {
            rebirthBonus.alreadyDisplayed = false;
            
        }
        if (GameData.totalFloorsCleared < 3)
        {
            foreach (RebirthBonus rebirthBonus in rebirthBonuses)
            {
                if (rebirthBonus.type == CharStat.xp)
                {
                    rebirthBonus.amount = CalculateRebirthBonus(CharStat.xp);
                }
            }
            return;
        }

        #region 1 - CALCULATE HOW MANY BONUS TYPES AVAILABLE
        int bonusCount = 2;
        // Floor 4,5,6 = offfense/mana
        if (GameData.totalFloorsCleared < 7 &&
           GameData.totalFloorsCleared > 3)
        {
            bonusCount = 3;
        }
        else  if (GameData.totalFloorsCleared < 10 &&
            GameData.totalFloorsCleared > 6)
        {
            bonusCount = 4;
        }
        // Floor 10+ = offense/defense/mana/life
        else if (GameData.totalFloorsCleared > 9)
        {
            bonusCount = 5;
        }
        #endregion

        #region 2 -  SELECT 2 random bonuses
        /*
         * 0 - xp
         * 1 - mana
         * 2 - offense
         * 3 - defense
         * 4 - life
         */
        int roll1 = Random.Range(1, bonusCount);
        int roll2 = roll1 + 1;
        if (roll2 == bonusCount)
            roll2 = 1;

        CharStat bonus1;
        CharStat bonus2;

        if (bonusCount == 2)
        {
            bonus1 = CharStat.xp;
            bonus2 = CharStat.mana;
        }
        else if (bonusCount == 3)
        {
            bonus1 = CharStat.mana;
            bonus2 = CharStat.offense;
        }
        else
        {
             bonus1 = GetCharStat(roll1);
             bonus2 = GetCharStat(roll2);
        }

        
        #endregion

        #region 3 - CALCULATE effect of chosen bonuses
        foreach(RebirthBonus curBonus in rebirthBonuses)
        {
            if (curBonus.type == bonus1 || curBonus.type == bonus2)
                curBonus.amount = CalculateRebirthBonus(curBonus.type);
        }
        #endregion 

    }

    /// <summary>
    /// Returns charstat depending on randomly given dice
    ///     * 0 - xp
    ///     * 1 - mana
    ///     * 2 - offense
    ///     * 3 - defense
    ///     * 4 - life
    /// </summary>
    /// <param name="dice"></param>
    /// <returns></returns>
    private CharStat GetCharStat(int dice)
    {
        CharStat newCharStat = CharStat.mana;
        switch(dice)
        {
            case 0:
                newCharStat = CharStat.xp;
                break;
            case 1:
                newCharStat = CharStat.mana;
                break;
            case 2:
                newCharStat = CharStat.offense;
                break;
            case 3:
                newCharStat = CharStat.defense;
                break;
            case 4:
                newCharStat = CharStat.life;
                break;

        }
        return newCharStat;
    }

    /// <summary>
    /// 
    /// 1,2 = XP BONUS 
    /// 3 = XP BONUS OR 1 MANA
    /// 4, 5, 6 = 1 OFFENSE OR 1 MANA
    /// 7, 9 = 2 offfense/defense/mana
    /// 10+n = 3+n/2 offense/defense/mana/life
    /// 
    /// </summary>
    private int CalculateRebirthBonus(CharStat charStat)
    {
        Debug.Log("CALCULATING REBIRTH");
        int amount = 0;
        bool hasRebirthPenalty = false;
        int realFloorReached = GameData.current.dungeonFloor + 2;
        Debug.Log("REAL FLOOR " + realFloorReached + ". MAX FLOOR: " + GameData.maxFloorReached);
        if (realFloorReached <= GameData.maxFloorReached)
        {
            hasRebirthPenalty = true;
            
        }
        GameData.current.UpdateMaxFloorReached();
        if (charStat == CharStat.xp)
        {
            amount = realFloorReached * 2;
            if (hasRebirthPenalty)
                amount = realFloorReached;
        }
        else
        {
            if (realFloorReached < 7)
                amount = 1;
            else if (realFloorReached < 10)
                amount = 2;
            else
                amount = 3 + (int)((realFloorReached - 10) * 0.5f);
            if (hasRebirthPenalty && amount > 1)
                amount--;

        }
        return amount;
    }

    public RebirthBonus GetRebirthBonusInfo()
    {
        RebirthBonus rebirthBonus = rebirthBonuses[0];

        foreach (RebirthBonus curBonus in rebirthBonuses)
        {
            if (!curBonus.alreadyDisplayed && curBonus.amount > 0)
            {
                curBonus.alreadyDisplayed = true;
                    return curBonus;
            }
        }
        return rebirthBonus;
    }

    public void ApplyRebirthBonus(PlayerControls player)
    {
        Debug.Log("applying REBIRTH");
        foreach (RebirthBonus rebirthBonus in rebirthBonuses)
        {
                switch (rebirthBonus.type)
                {
                    case CharStat.xp:
                        player.stats.currExp += rebirthBonus.hiddenAmount;
                        Debug.Log(" XP REBIRTH " + rebirthBonus.hiddenAmount);
                        break;
                    case CharStat.mana:
                        player.stats.currMana += rebirthBonus.hiddenAmount;
                        player.stats.maxMana += rebirthBonus.hiddenAmount;
                        Debug.Log(" MANA REBIRTH" + rebirthBonus.hiddenAmount);
                        break;
                    case CharStat.offense:
                        player.stats.offense += rebirthBonus.hiddenAmount;
                        Debug.Log(" OFF REBIRTH " + rebirthBonus.hiddenAmount);
                        break;
                    case CharStat.defense:
                        player.stats.defense += rebirthBonus.hiddenAmount;
                        Debug.Log(" DEF REBIRTH " + rebirthBonus.hiddenAmount);
                        break;
                    case CharStat.life:
                        player.stats.currLife += rebirthBonus.hiddenAmount;
                        player.stats.maxLife += rebirthBonus.hiddenAmount;
                        Debug.Log(" LIF REBIRTH " + rebirthBonus.hiddenAmount);
                        break;
                }

        }
    }
}
