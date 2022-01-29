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

public class CharacterStats
{
    // level
    public int level;
    public int currExp;
    public int expRequired; // xp required ot lv up

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

        switch (newCharacter)
        {
            case Character.Luna:
                GenerateVarna();
                break;
            case Character.Crow:
                GenerateSiksparnis();
                break;
            case Character.Dog:
                GenerateNave();
                break;
            default:
                Debug.Log("CHARACTER NOT DEFINIED");
                break;
        }
    }

    private void GenerateVarna()
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

    private void GenerateSiksparnis()
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
    
    
    private void GenerateNave()
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
}
