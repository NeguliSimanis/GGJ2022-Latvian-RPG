using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Skill[] allSkills;

    public Skill GetSkill(string skillName)
    {
        
        foreach (Skill currSkill in allSkills)
        {
            if (currSkill.skillName == skillName)
                return currSkill;
        }
        Debug.LogError("SKILL NOT FOUND " + skillName);
        return null;
    }
}
