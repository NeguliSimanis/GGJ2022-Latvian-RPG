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
}
