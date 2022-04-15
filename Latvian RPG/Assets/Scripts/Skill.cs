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
    public int skillID = 0;
    public float skillDamage;
    public float manaCost;
    public string skillName;
    public int skillRange;
    public SkillType[] type;
    public string description;
    public SkillEffectObject[] skillEffects;
    public GameObject skillAnimation;
    public Vector2 animationOffset;

    public string GetDescription()
    {
        string totalDescription;

        if (type[0] == SkillType.Recruit)
        {
            totalDescription = description 
                 + "\n\nMana cost: " + (int)manaCost
                + "\nRange: " + skillRange;
        }
        else if (type[0] == SkillType.Buff)
        {
            totalDescription = description
                 + "\n\nMana cost: " + (int)manaCost
                + "\nRange: " + skillRange;
        }
        else
        {
            totalDescription = description
                + "\n \nBase damage: " + (int)skillDamage
                + "\nMana cost: " + (int)manaCost
                + "\nRange: " + skillRange;
        }
         
        return totalDescription;
    }

    public string GetCharPanelDescription()
    {

        string totalDescription = skillName.ToUpper() + ": " + description;

        if (type[0] == SkillType.Recruit)
        {
            totalDescription +=
                "\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }
        else if (type[0] == SkillType.Buff)
        {
            totalDescription += "\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }
        else
        {
            totalDescription += "\n Base damage: " + (int)skillDamage
                + "\n Mana cost: " + (int)manaCost
                + "\n Range: " + skillRange;
        }

        return totalDescription;
    }


    public void ApplySkillEffects(PlayerControls target)
    {
        foreach (SkillEffectObject skillEffect in skillEffects)
        {
            GameObject newEffectObject = Instantiate(skillEffect.gameObject);
            SkillEffectObject newSkillEffectObj = newEffectObject.GetComponent<SkillEffectObject>();
            SkillEffect newSkillEffect = newSkillEffectObj.GetSkillEffect();
            target.stats.activeStatusEffects.Add(newSkillEffect);
            Destroy(newSkillEffectObj);
            Debug.Log("Added " + skillEffect.name + " skill effect to " + target.name);
            target.ActivateStatusEffects();
        }
    }
}
