using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum CharStatus
{
    Undefined,
    Guarded,
}

[Serializable]
public class SkillEffect
{
    public CharStatus inflictedStatus;

    public int effectDuration;

    #region ARMOR MODIF
    [Header("ARMOR")]
    public float armorPercentIncrease;
    public int armorIncrease;
    public int increasedArmor;
    public bool armorIncreased = false;
    #endregion

    #region MANA MODIF
    [Header("MANA")]
    public int manaIncrease;
    #endregion

    public bool canTargetOnlySelf;
    

    public string GetSkillEffectDescr()
    {
        string descr = inflictedStatus.ToString().ToUpper() + ": ";

        switch (inflictedStatus)
        {
            case CharStatus.Guarded:
                descr += "Armor increased by " + armorIncrease + " for " + effectDuration + " turns";
                break;
        }
        return descr;
    }
}



public class SkillEffectObject : MonoBehaviour
{
    [SerializeField]
    CharStatus inflictedStatus;

    public int effectDuration;

    #region ARMOR MODIF
    [Header("ARMOR")]
    public float armorPercentIncrease;
    public int armorIncrease;
    public int increasedArmor;
    public bool armorIncreased = false;
    #endregion

    #region MANA MODIF
    [Header("MANA")]
    public int manaIncrease;
    #endregion

    public bool canTargetOnlySelf;

    public SkillEffect GetSkillEffect()
    {
        SkillEffect newSkillEffect = new SkillEffect();

        newSkillEffect.inflictedStatus = inflictedStatus;
        newSkillEffect.effectDuration = effectDuration;

        // ARMOR
        newSkillEffect.armorPercentIncrease = armorPercentIncrease;
        newSkillEffect.armorIncrease = armorIncrease;
        newSkillEffect.increasedArmor = increasedArmor;
        newSkillEffect.armorIncreased = armorIncreased;

        // MANA
        newSkillEffect.manaIncrease = manaIncrease;

        return newSkillEffect;
    }
}
