using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    GameManager gameManager;

    
    [HideInInspector]
    public bool isWarningPopupActive = false;

    [SerializeField]
    GameObject popupObject;
    [SerializeField]
    Button confirmEndTurnButton;
    [SerializeField]
    Button cancelEndTurnButton;

    [SerializeField]
    GameObject gameLostPanel;
    [SerializeField]
    Button restartGameButton;

    [Header("Character popup")]
    [SerializeField]
    GameObject charPopup;
    [SerializeField]
    Button openCharPopupButton;
    [SerializeField]
    Button closeCharPopupButton;

    // flair
    [SerializeField]
    Text charName;
    [SerializeField]
    Text charBioText;
    [SerializeField]
    Image charPopupImage;

    // stats
    [SerializeField]
    Text expText;

    [SerializeField]
    Text offenseText;
    [SerializeField]
    Text armorText;

    [SerializeField]
    Text lifeText;
    [SerializeField]
    Text manaText;

    [SerializeField]
    Text speedText;



    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        ShowWarningPopup(true);

        confirmEndTurnButton.onClick.AddListener(ConfirmEndTurn);
        cancelEndTurnButton.onClick.AddListener(CancelEndTurn);
        restartGameButton.onClick.AddListener(RestartGame);

        ShowWarningPopup(false);
        DisplayGameLostPopup(false);
        ShowCharPopup( new PlayerControls(), false);

    }

    public void ShowWarningPopup(bool show)
    {
        isWarningPopupActive = show;
        popupObject.SetActive(show);
    }

    private void ConfirmEndTurn()
    {
        gameManager.EndTurn();
    }

    private void CancelEndTurn()
    {
        ShowWarningPopup(false);
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
    }

    public void DisplayGameLostPopup(bool show = true)
    {
        if (!show)
        {
            gameLostPanel.SetActive(false);
            return;
        }
        gameLostPanel.SetActive(true);
    }


    public void ShowCharPopup(PlayerControls playerControls, bool show = true)
    {
        if (!show)
        {
            charPopup.SetActive(false);
            return;
        }
        charPopup.SetActive(true);

        /// REPLACE THIS STUFF WITH TEXTMESHJ
        charName.text = playerControls.stats.name;
         charBioText.text = playerControls.stats.bio;
         //charPopupImage.image;

        // stats
         expText.text = "exp: " + playerControls.stats.currExp.ToString();

         offenseText.text = "Offense: " + playerControls.stats.offense.ToString();
         armorText.text = "Defense: " + playerControls.stats.defense.ToString();

         lifeText.text = "Life: " + playerControls.stats.currLife.ToString() + "/" + playerControls.stats.maxLife.ToString();
         manaText.text = "Mana: " + playerControls.stats.currMana.ToString() + "/" + playerControls.stats.maxMana.ToString();

        speedText.text = "Speed: " + playerControls.stats.speed;
    }
}
