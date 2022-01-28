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

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        ShowWarningPopup(true);

        confirmEndTurnButton.onClick.AddListener(ConfirmEndTurn);
        cancelEndTurnButton.onClick.AddListener(CancelEndTurn);
        restartGameButton.onClick.AddListener(RestartGame);

        ShowWarningPopup(false);
        DisplayGameLostPopup(false);

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
}
