using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthBonus
{
    public CharStat type;
    public int amount;
    public bool alreadyDisplayed = false;

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


    /// <summary>
    /// 
    /// Rolls what rebirth bonuses will be available.
    /// The bonuses depend on how many floors have been cleared:
    /// 1,2 = XP BONUS
    /// 3,4 = XP BONUS OR MANA
    /// 5,6 = OFFENSE OR MANA
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

        foreach (RebirthBonus rebirthBonus in rebirthBonuses)
        {
            // 1,2 = XP BONUS
            if (GameData.totalFloorsCleared < 3)
            {

                return;
            }
            // 3,4 = XP BONUS OR MANA
            else if (GameData.totalFloorsCleared < 5)
            {


                return;
            }

            // 5,6 = OFFENSE OR MANA
            if (GameData.totalFloorsCleared < 7)
            {
            }
            // 7, 9 = offfense/defense/mana
            else if (GameData.totalFloorsCleared < 10)
            {
            }
            // 10+ = offense/defense/mana/life
            else if (GameData.totalFloorsCleared > 9)
            {
            }
        }
    }

    private int CalculateRebirthBonus(CharStat charStat)
    {
        int amount = 0;
        switch(charStat)
        {
            case CharStat.xp:
                amount = GameData.totalFloorsCleared * 2;
                break;
        }
        return amount;
    }
}
