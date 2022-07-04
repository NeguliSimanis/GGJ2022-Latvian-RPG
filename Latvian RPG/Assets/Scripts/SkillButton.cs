using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler
{
    bool skillInitialized = false;

    [HideInInspector]
    public PopupManager popupManager;
    public Skill skill;

    [SerializeField]
    GameObject helpObject;
    [SerializeField]
    Text helpText;
    [SerializeField]
    Image skillIcon;
    public Button skillInfoButt;

    public Text skillButtonText;
    public Image skillButtonImage;
    public Button thisButton;

    Color skillIconDefColor = Color.white;
    Color skillIconSelectedColor = Color.black;
    int touchCountWhenOpenedSkillInfo = 0;


    private void Start()
    {
        skillInfoButt.gameObject.SetActive(true);
        skillInfoButt.onClick.AddListener(() => ShowSkillInfo());
        skillInfoButt.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        // hide skill info if u click anywhere while it's open
        if (GameData.current.gameStarted && GameData.current.turnType == CharType.Player)
        {
            if (popupManager.skillExplanation.gameObject.activeInHierarchy)
            {
                if (Input.touchCount > 0 || Input.GetKey(0))
                {
                    Debug.LogError("HOOOO");
                    ShowSkillInfo(); 
                }
            }
        }
    }

    public void ShowSkillInfo(bool show = true)
    {
        Debug.LogError("im here baby");
        if (popupManager.skillExplanation.gameObject.activeInHierarchy) 
            //&& popupManager.skillExplanation.currSkill.skillName == skill.skillName)
            show = false;
        if (!show)
        {
            //helpObject.SetActive(show);
            popupManager.skillExplanation.gameObject.SetActive(show);
            skillInitialized = false;
            return;
        }
        //if (skillInitialized && )
        //{
        //    return;
        //}
        touchCountWhenOpenedSkillInfo = Input.touchCount;
        //  helpObject.SetActive(show);
        popupManager.skillExplanation.gameObject.SetActive(show);
        popupManager.skillExplanation.skillDescr.text = skill.GetDescription();
        popupManager.skillExplanation.skillIcon.sprite = skill.skillIcon;
        popupManager.skillExplanation.skillNameText.text = skill.skillName.ToUpper();
        popupManager.skillExplanation.currSkill = skill;
        ///helpText.text = skill.GetDescription();
        skillInitialized = true;
    }

    public void UpdateSkillIcon(Sprite newIcon)
    {
        skillIcon.sprite = newIcon;
    }

    public void ColorSkillIcon(Color newColor)
    {
        
       skillIcon.color = newColor;

        //Debug.LogError(Time.time + " " + "color all: " + colorAll);
    }

    private void IsMouseOverButton()
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //}
        //else
        //    ShowSkillInfo();
    }

    /// <summary>
    /// Displays skill info when cursor over button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR || UNITY_ANDROID
        return;
#endif

#if UNITY_STANDALONE_WIN
        if (GameData.current.gameStarted && GameData.current.turnType == CharType.Player)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.name == gameObject.name)
                {
                    //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                    // if (EventSystem.current.currentSelectedGameObject.name == gameObject.name)
                    //  Debug.Log(gameObject.name);
                    ShowSkillInfo(true);
                }
                else
                    ShowSkillInfo(false);
            }
        }
        else
        {
            ShowSkillInfo(false);
        }
#endif

    }

}
