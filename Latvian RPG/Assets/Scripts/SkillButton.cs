using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler
{
    bool skillInitialized = false;

    public Skill skill;

    [SerializeField]
    GameObject helpObject;
    [SerializeField]
    Text helpText;

    public Text skillButtonText;
    public Image skillButtonImage;
    public Button thisButton;

    private void Start()
    {
        ShowSkillInfo(false);
    }

    private void Update()
    {
        if (GameData.current.gameStarted && GameData.current.turnType == CharType.Player)
        {
            IsMouseOverButton();
        }
    }

    private void ShowSkillInfo(bool show)
    {
        if (!show)
        {
            helpObject.SetActive(show);
            skillInitialized = false;
            return;
        }
        if (skillInitialized)
            return;
        helpObject.SetActive(show);
        helpText.text = skill.GetDescription();
        skillInitialized = true;
    }

    private void IsMouseOverButton()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
        }
        else
            ShowSkillInfo(false);
    }

    /// <summary>
    /// Displays skill info when cursor over button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
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

        
    }

}
