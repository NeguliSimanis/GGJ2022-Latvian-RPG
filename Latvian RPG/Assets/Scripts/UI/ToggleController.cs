using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToggleController : MonoBehaviour
{
    public GameManager gameManager;
    public bool isOffToggle;
    public Toggle otherToggle;
    private Toggle thisToggle;

    private void OnEnable()
    {
        thisToggle = gameObject.GetComponent<Toggle>();
        SetCorrectToggleState();
    }

    private void SetCorrectToggleState()
    {
        if (isOffToggle)
        {
            // Toggle is incorrect
            if (thisToggle.isOn && GameData.current.hasTurnDuration)
            {
                thisToggle.SetIsOnWithoutNotify(false);
            }
            else if (!thisToggle.isOn && !GameData.current.hasTurnDuration)
            {
                thisToggle.SetIsOnWithoutNotify(true);
            }
        }
        else
        {
            if (thisToggle.isOn && !GameData.current.hasTurnDuration)
            {
                thisToggle.SetIsOnWithoutNotify(false);
            }
            else if (!thisToggle.isOn && GameData.current.hasTurnDuration)
            {
                thisToggle.SetIsOnWithoutNotify(true);
            }
        }
    }

    public void ToggleTurnDuration()
    {
        if (isOffToggle)
        {
            // TURN OFF TURN DURATION
            if (thisToggle.isOn)
            {
                EnableTurnDuration(false);
            }
            // TURN ON TURN DURATION
            else
            {
                EnableTurnDuration();
            }
        }
        else
        {
            // TURN ON TURN DURATION
            if (thisToggle.isOn)
            {
                EnableTurnDuration();
            }
            // TURN OFF TURN DURATION
            else
            {
                EnableTurnDuration(false);
            }
        }
    }

    private void EnableTurnDuration(bool enable = true)
    {
        GameData.current.hasTurnDuration = enable;
        gameManager.ShowTurnTimerBar(enable);
        
        // change the other toggle without calling the related methods
        otherToggle.SetIsOnWithoutNotify(!thisToggle.isOn);
    }

}
