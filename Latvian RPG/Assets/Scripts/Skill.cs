using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Damage,
    Recruit,
    Buff,
    Debuff
}

public class Skill : MonoBehaviour
{
    public float skillDamage;
    public float manaCost;
    public string skillName;
    public int skillRange;
    public SkillType[] type;
    public string description;
    public SkillEffect[] skillEffects;

    public string GetDescription()
    {
        string totalDescription;

        if (type[0] == SkillType.Recruit)
        {
            totalDescription = description 
                 + "\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }
        else if (type[0] == SkillType.Buff)
        {
            totalDescription = description
                 + "\n\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }
        else
        {
            totalDescription = description
                + "\n \n Base damage: " + (int)skillDamage
                + "\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }
         
        return totalDescription;
    }
}
