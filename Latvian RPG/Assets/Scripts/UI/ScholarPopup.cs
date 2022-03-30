using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScholarPopup : MonoBehaviour
{

    public Text skillText;
    public Text scholarText;
    public Text skillName;

    [Header("Light option")]
    public Button lightOptionButton;
    public Text lightBigText;
    public Text lightSmallText;

    [Header("Dark option")]
    public Button darkOptionButton;
    public Text darkBigText;
    public Text darkSmallText;

    [HideInInspector]
    public Skill skillToTeach;
}
