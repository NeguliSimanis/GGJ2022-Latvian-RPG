using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebirthStatButton : MonoBehaviour
{
    [SerializeField]
    Image statImage;

    [SerializeField]
    Text statAmount;

    [SerializeField]
    Text statName;

    Button thisButton;

    public void DisplayStats(CharStat rebirthStat, bool isXP, PopupManager popupManager, int amount)
    {
        Debug.Log("displaying stats");
        thisButton = gameObject.GetComponent<Button>();
        statAmount.text = amount.ToString();
        if (isXP)
        {
            statImage.sprite = popupManager.xpIcon;
            statName.text = "experience";
            Debug.Log("THIS BUTT GIVES XP");
            return;
        }
        switch (rebirthStat)
        {
            case CharStat.defense:
                statName.text = "defense";
                statImage.sprite = popupManager.defenseIcon;
                Debug.Log("THIS BUTT GIVES DEFENSE");
                break;
            case CharStat.offense:
                statImage.sprite = popupManager.offenseIcon;
                Debug.Log("THIS BUTT GIVES OFFESN");
                statName.text = "offense";
                break;
            case CharStat.life:
                statName.text = "life";
                statImage.sprite = popupManager.lifeIcon;
                Debug.Log("THIS BUTT GIVES LIFE");
                break;
            case CharStat.mana:
                statName.text = "mana";
                statImage.sprite = popupManager.manaIcon;
                Debug.Log("THIS BUTT GIVES MANA");
                break;
        }
    }
}
