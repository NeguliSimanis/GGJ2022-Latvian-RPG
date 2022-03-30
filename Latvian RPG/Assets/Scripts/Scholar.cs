using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scholar : MonoBehaviour
{
    [SerializeField]
    Skill []allowedSkills;

    public Skill SelectSkillToTeach(PlayerControls apprentice)
    {
        

        // CREATE A LIST OF SKILLS TO CHOOSE FROM
        List<Skill> availableSkills = new List<Skill>();
        foreach (Skill skill in allowedSkills)
        {
            bool available = true;
            foreach (Skill apprenticeSkills in apprentice.stats.skills)
            {
                if (skill.name == apprenticeSkills.name)
                {
                    available = false;
                    break;
                }
            }
            if (available)
                availableSkills.Add(skill);
        }

        // roll a random skill from available
        int skillCount = availableSkills.Count;



        int skillRoll = Random.Range(0, skillCount);
        Skill skillToTeach = availableSkills[skillRoll];
        return skillToTeach;
    }
}
