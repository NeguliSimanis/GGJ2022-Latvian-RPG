using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public int effectDuration;

    #region ARMOR MODIF
    [Header("ARMOR")]
    public float armorPercentIncrease;
    public int originalArmor;
    public int increasedArmor;
    public bool armorIncreased = false;
    #endregion

    #region MANA MODIF
    [Header("MANA")]
    public int manaIncrease;
    #endregion

    public bool canTargetOnlySelf;
}
