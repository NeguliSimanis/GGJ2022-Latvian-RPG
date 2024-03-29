﻿using System.Collections;
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
    [SerializeField]
    Image skillIcon;
    public Button skillInfoButt;

    public Text skillButtonText;
    public Image skillButtonImage;
    public Button thisButton;

    Color skillIconDefColor = Color.white;
    Color skillIconSelectedColor = Color.black;


    private void Start()
    {
        ShowSkillInfo(false);
        skillInfoButt.gameObject.SetActive(true);
        skillInfoButt.onClick.AddListener(() => ShowSkillInfo(true));
        skillInfoButt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameData.current.gameStarted && GameData.current.turnType == CharType.Player)
        {
            IsMouseOverButton();
        }
    }

    public void ShowSkillInfo(bool show = true)
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
