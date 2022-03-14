using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    [Header("TURN MANAGEMENT")]
    [SerializeField]
    private Animator turnAnimator;
    [SerializeField]
    private GameObject turnAnimatorObject;
    [SerializeField]
    private Text turnText;

    private float turnTextDuration = 2f;
    private GameManager gameManager;


    private void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        turnAnimatorObject.SetActive(false);
    }

    public void StartNewTurn()
    {
        GameData.current.currentTurn++;
        string newTurnText = "Default Turn Text";

        switch (GameData.current.turnType)
        {
            case CharType.Player:
                GameData.current.turnType = CharType.Enemy;
                Debug.Log("start enemy turn");
                newTurnText = "Enemy Turn";
                break;
            case CharType.Enemy:
                GameData.current.turnType = CharType.Neutral;
                Debug.Log("start neutral turn");
                newTurnText = "Neutral Turn";
                break;
            case CharType.Neutral:
                GameData.current.turnType = CharType.Player;
                Debug.Log("start player turn");
                newTurnText = "Your Turn";
                break;
        }

        gameManager.popupManager.UpdateTurnText();
        //StartCoroutine(ShowTurnTextForSeconds(newTurnText));
    }

    private IEnumerator ShowTurnTextForSeconds(string newTurnText)
    {
        turnAnimatorObject.SetActive(true);
        turnText.text = newTurnText;
        yield return new WaitForSeconds(turnTextDuration);
        turnAnimatorObject.SetActive(false);
    }
    
}
