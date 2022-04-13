using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scholar : MonoBehaviour
{
    [SerializeField]
    Skill []allowedSkills;

    public void SelectSkillsToTeach(PlayerControls apprentice)
    {
        

        // CREATE A LIST OF SKILLS TO CHOOSE FROM
        //List<Skill> availableSkills = new List<Skill>();
        apprentice.scholarOfferedSkills.Clear();


        foreach (Skill skill in allowedSkills)
        {
            bool available = true;
            foreach (Skill apprenticeSkills in apprentice.currentSkills)
            {
                if (skill.name == apprenticeSkills.name)
                {
                    available = false;
                    break;
                }
            }
            if (available)
                apprentice.scholarOfferedSkills.Add(skill);
        }

        // roll a random skill from available
        int skillCount = apprentice.scholarOfferedSkills.Count;




        int skillRoll = Random.Range(0, skillCount);
        Skill skillToTeach = apprentice.scholarOfferedSkills[skillRoll];
        
    }
}
