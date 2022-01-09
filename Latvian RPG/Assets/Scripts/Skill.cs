using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Damage,
    Recruit
}

public class Skill : MonoBehaviour
{

    public float skillDamage;
    public float skillCost;
    public string skillName;
    public int skillRange;
    public SkillType type;
}
