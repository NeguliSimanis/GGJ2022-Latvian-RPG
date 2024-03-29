﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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
    Dog,
    Wolf,
    Goat,
    Undefined
}

public enum CharStat
{
    offense,
    defense,
    speed,
    life,
    mana,
    xp
}

[Serializable]
public class CharacterStats
{
    // data for saving
    public CharType savedCharType; // player/neutral/enemy
    public float lastSavedPosX; // only used for saving/loading
    public float lastSavedPosY; // only used for saving/loading
    public Character savedCharacter;
    public bool savedIsDead;

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

    public int tilesWalked = 0;
    public int speed; // number of tiles that char can walk in single turn
    
    public List<string> skills;

    // flair
    public Alignment alignment;
    public string bio;
    public string name;

    public List<SkillEffect> activeStatusEffects = new List<SkillEffect>();

    public void ChangeWalkedTiles(int change = 0, bool reset = false, [CallerMemberName] string callerName = "")
    {
        Debug.Log("called " + callerName);
        tilesWalked += change;
        if (reset)
            tilesWalked = 0;
    }

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
            case Character.Wolf:
                GenerateWolf();
                break;
            case Character.Goat:
                GenerateGoat();
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

        skills = new List<string>();

        // flair
        alignment = Alignment.Piety;
        bio = "She used to be a queen beloved by her people!" +
            "Benevolent and righteous, she was." +
            "Ah, how quickly the righteous turn on their friends and slaughter them." +
            "";
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

        skills = new List<string>();

        // flair
        alignment = Alignment.Might;
        bio = "They say after the battle of Tripeak, there were no survivors." +
            " This is false.Before you is the child that survived the flames and blades." +
            " But will it survive you ?";
        name = "Koyanagi";
    }

    private void GenerateWolf()
    {
        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<string>();

        // flair
        alignment = Alignment.Might;
        bio = "Take heed, a great general is before you!" +
            " His armies dead, his land barren and his children murdered by their mother." +
            " For even a great general can be defeated, if a God craves for a tragedy.";
        name = "Cellach";
    }



    private void GenerateGoat()
    {
        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<string>();

        // flair
        alignment = Alignment.Might;
        bio = "Oh, you found one of my favorites!" +
            " Many are those, who reach me through their might or cunning, great people of war and politics." +
            " This one raised entire forests and gave shelter to men and beast alike, after a calamity struck their land." +
            "";
        name = "Zurabi";
    }
    private void GenerateDog()
    {
        // combat
        maxLife = 10;
        currLife = maxLife;

        maxMana = 10;
        currMana = maxMana;

        speed = 2; // number of tiles that char can walk in single turn

        skills = new List<string>();

        // flair
        alignment = Alignment.Might;
        bio = "Oh, I remember him! His true death was quite the spectacle. " +
            "He stood alone against an entire army and held that bridge for three days!" +
            "It took seventeen arrows to take him down, what a man!";
        name = "Winston";
    }

    public int GetStatIncreaseAmount(CharStat stat)
    {
        int amount = 0;
        switch (stat)
        {
            case CharStat.life:
                amount = UnityEngine.Random.Range(2, 4);
                break;
            case CharStat.offense:
                amount = UnityEngine.Random.Range(1, 3);
                break;
            case CharStat.defense:
                amount = UnityEngine.Random.Range(1, 3);
                break;
            case CharStat.mana:
                amount = UnityEngine.Random.Range(2, 4);
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

        Debug.Log("completed dark action " + isDarkAction + " worth " + amount);
        if (GameData.current.currMoonPoints >= GameData.current.pointsRequiredPhase3 ||
            GameData.current.currMoonPoints <= -GameData.current.pointsRequiredPhase3)
        {
            return true;
        }
        return false;
    }
}
