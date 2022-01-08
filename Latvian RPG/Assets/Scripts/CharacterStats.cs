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
    Varna
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

    public int offense;
    public int defense;

    public int iniative; // how quickly will the character act in turns
    public int speed; // number of tiles that char can walk in single turn
    
    public List<Skill> skills;

    // flair
    public Alignment alignment;
    public string bio;
    public string name;

    public CharacterStats (Character newCharacter)
    {
        switch (newCharacter)
        {
            case Character.Varna:
                GenerateVarna();
                break;
            default:
                Debug.Log("CHARACTER NOT DEFINIED");
                break;
        }
    }

    private void GenerateVarna()
    {
        level = 1;
        currExp = 0;
        expRequired = GameData.current.defaultLevelExp; // xp required ot lv up

        // combat
        maxLife = 100;
        currLife = maxLife;

        maxMana = 100;
        currMana = maxMana;

        offense = 10;
        defense = 6;

        iniative = 10; // how quickly will the character act in turns
        speed = 5; // number of tiles that char can walk in single turn

        skills = new List<Skill>();

        // flair
        alignment = Alignment.Piety;
        bio = "Half-woman, half-crow, Varna roams the lands in search of vengeance";
        name = "Varna";
    }
}
