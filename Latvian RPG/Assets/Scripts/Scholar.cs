﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scholar : MonoBehaviour
{
    [SerializeField]
    Skill []allowedSkills;

    public int SelectSkillsToTeach(PlayerControls apprentice)
    {
        int skillsToTeach = 0;

        // CREATE A LIST OF SKILLS TO CHOOSE FROM
        List<Skill> availableSkills = new List<Skill>();
        apprentice.scholarOfferedSkills.Clear();

        if (apprentice.currentSkills.Count >= 4)
            ChooseSkillToForget(apprentice);

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
                availableSkills.Add(skill);
        }

        // roll a random skill from available
        int skillCount = availableSkills.Count;
        int selectedSkillCount = 0;
        int safetyCounter = 30;

        while (selectedSkillCount < 2 && safetyCounter > 0)
        {
            int skillRoll = Random.Range(0, skillCount);
            Skill skillToTeach = availableSkills[skillRoll];
            if (apprentice.scholarOfferedSkills.Count < 1 ||
                apprentice.scholarOfferedSkills[0].skillName != skillToTeach.skillName)
            {
                apprentice.scholarOfferedSkills.Add(skillToTeach);
                skillsToTeach++;
                selectedSkillCount++;
            }
            safetyCounter--;
        }
        return skillsToTeach;
    }

    public void ChooseSkillToForget(PlayerControls apprentice)
    {
        int skillCount = apprentice.currentSkills.Count;
        int randomRoll = Random.Range(0, skillCount);
        apprentice.skillToForget = apprentice.currentSkills[randomRoll];
    }

}
