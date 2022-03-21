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


    public void DisplayStats(RebirthBonus rebirthBonus, PopupManager popupManager)
    {
        thisButton = gameObject.GetComponent<Button>();
        thisButton.onClick.AddListener((delegate { SelectRebirthBonus(rebirthBonus); }));
        statAmount.text = "+" + rebirthBonus.amount.ToString();
        if (rebirthBonus.type == CharStat.xp)
        {
            statImage.sprite = popupManager.xpIcon;
            statName.text = "experience";
            return;
        }
        switch (rebirthBonus.type)
        {
            case CharStat.defense:
                statName.text = "defense";
                statImage.sprite = popupManager.defenseIcon;
                break;
            case CharStat.offense:
                statImage.sprite = popupManager.offenseIcon;
                statName.text = "offense";
                break;
            case CharStat.life:
                statName.text = "life";
                statImage.sprite = popupManager.lifeIcon;
                break;
            case CharStat.mana:
                statName.text = "mana";
                statImage.sprite = popupManager.manaIcon;
                break;
        }
    }

    public void SelectRebirthBonus(RebirthBonus rebirthBonus)
    {
        foreach (RebirthBonus rebirth in RebirthManager.instance.rebirthBonuses)
        {
            if (rebirth.type == rebirthBonus.type)
            {
                rebirthBonus.hiddenAmount += rebirthBonus.amount;
                Debug.Log("i have been chosen - " + rebirth.type + ". Amount: " + rebirth.hiddenAmount);
                rebirthBonus.amount = 0;
            }
        }
        GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>().RestartGame();
    }
}
