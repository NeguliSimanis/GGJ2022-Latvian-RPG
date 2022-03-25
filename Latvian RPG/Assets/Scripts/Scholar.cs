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
        int skillCount = allowedSkills.Length;
        int skillRoll = Random.Range(0, skillCount);
        Skill skillToTeach = allowedSkills[skillRoll];
        return skillToTeach;
    }
}
