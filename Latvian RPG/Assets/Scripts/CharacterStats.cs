using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment
{
    Piety,
    Wealth,
    Might,
}

public enum Character
{
    Luna,
    Crow,
    Dog
}

public enum CharStat
{
    offense,
    defense,
    speed,
    life,
    mana
}

public class CharacterStats
{
    // game progress
    public int lightPoints;
    public int darkPoints;

    // level
    public int level;
    public int currExp;
    public int expRequired; // xp required ot lv up

    public CharStat darkLevelUpStat;
    public CharStat lightLevelUpStat;

    // combat
    public float maxLife;
    public float currLife;

    public float maxMana;
    public float currMana;
    public float manaRegen;

    public int offense;
    public int defense;

    public int speed; // number of tiles that char can walk in single turn
    
    public List<Skill> skills;

    // flair
    public Alignment alignment;
    public string bio;
    public string name;

    public CharacterStats (Character newCharacter)
    {
        level = 1;
        currExp = 0;
        expRequired = GameData.current.defaultLevelExp; // xp required ot lv up

        offense = 10;
        defense = 10;

        manaRegen = 1;

        lightPoints = 0;
        darkPoints = 0;

        switch (newCharacter)
        {
            case Character.Luna:
                GenerateMedusa();
                break;
            case Character.Crow:
                GenerateCrow();
                break;
            case Character.Dog:
                GenerateDog();
                break;
            default:
                Debug.Log("CHARACTER NOT DEFINIED");
                break;
        }
    }

    private void GenerateMedusa()
    {

        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<Skill>();

        // flair
        alignment = Alignment.Piety;
        bio = "I swore to become her Umbral Champion or die trying. \n " +
            "And all throughout this time, she has done nothing short of killing me.\n " +
            "Years of incessant war have shattered my beauty and my soul.\n " +
            "The scales once so iridescent, now are as bleak as dust surrounding us.\n " +
            "I am covered in feathers, like a hideous, overgrown chicken.\n " +
            "And the joy of combat and screams of the dying sooth my soul no more.\n " +
            "She uses each waking moment to tear me down and corrupt me.\n ";
        name = "Luna";
    }

    private void GenerateCrow()
    {
        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<Skill>();

        // flair
        alignment = Alignment.Might;
        bio = "Angry bat boy";
        name = "Siksparnis";
    }    
    
    
    private void GenerateDog()
    {
        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<Skill>();

        // flair
        alignment = Alignment.Might;
        bio = "Avatar of Death";
        name = "Nave";
    }

    public int GetStatIncreaseAmount(CharStat stat)
    {
        int amount = 0;
        switch (stat)
        {
            case CharStat.life:
                amount = Random.Range(2, 4);
                break;
            case CharStat.offense:
                amount = Random.Range(1, 3);
                break;
            case CharStat.defense:
                amount = Random.Range(1, 3);
                break;
            case CharStat.mana:
                amount = Random.Range(2, 4);
                break;
            case CharStat.speed:
                amount = 1;
                break;
        }
        return amount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expAction"></param>
    /// <returns>true if game won</returns>
    public bool UpdateProgressToGameVictory(ExpAction expAction)
    {
        int amount = 0;
        bool isDarkAction = true;
        switch(expAction)
        {
            case ExpAction.Heal:
                amount = GameData.current.healPointsReward;
                isDarkAction = false;
                break;
            case ExpAction.Hire:
                amount = GameData.current.recruitPointsReward;
                isDarkAction = false;
                break;
            case ExpAction.Kill:
                amount = GameData.current.killPointsReward;
                isDarkAction = true;
                break;
            case ExpAction.LevelUpDark:
                amount = GameData.current.levelUpPointsReward;
                isDarkAction = true;
                break;
            case ExpAction.LevelUpLight:
                amount = GameData.current.levelUpPointsReward;
                isDarkAction = false;
                break;
        }
        if (isDarkAction)
        {
            GameData.current.currMoonPoints -= amount;
        }
        else
            GameData.current.currMoonPoints += amount;

        if (GameData.current.currMoonPoints >= GameData.current.pointsRequiredPhase3 ||
            GameData.current.currMoonPoints <= -GameData.current.pointsRequiredPhase3)
        {
            return true;
        }
        return false;
    }
}
