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
            return;
        }
        switch (rebirthStat)
        {
            case CharStat.defense:
                break;
        }
    }
}
