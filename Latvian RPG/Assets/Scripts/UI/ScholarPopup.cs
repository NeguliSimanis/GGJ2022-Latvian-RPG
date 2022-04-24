using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScholarPopup : MonoBehaviour
{
    public Text scholarText;
    public GameObject hasSkillsPanel;

    [Header("Light option")]
    public Button lightOptionButton;
    public Text lightButtonText;
    public Text lightBigText;
    public Text lightSmallText;

    public Text lightSkillName;
    public Text lightSkillText;

    [Header("Dark option")]
    public Button darkOptionButton;
    public Text darkButtonText;
    public Text darkBigText;
    public Text darkSmallText;

    public Text darkSkillName;
    public Text darkSkillText;

    [HideInInspector]
    public Skill lightSkillToTeach;
    [HideInInspector]
    public Skill darkSkillToTeach;
    [HideInInspector]
    public Skill skillToForget;

    [Header("No skills option")]
    public Button noSkillsButton;
    public GameObject noSkillsPanel;
}
