using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScholarPopup : MonoBehaviour
{

   
    public Text scholarText;

    

    [Header("Light option")]
    public Button lightOptionButton;
    public Text lightBigText;
    public Text lightSmallText;

    public Text lightSkillName;
    public Text lightSkillText;

    [Header("Dark option")]
    public Button darkOptionButton;
    public Text darkBigText;
    public Text darkSmallText;

    public Text darkSkillName;
    public Text darkSkillText;

    [HideInInspector]
    public Skill skillToTeach;
}
