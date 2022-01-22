using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnType
{
    Player,
    Enemy,
    Neutral
}

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

    private void Start()
    {
        turnAnimatorObject.SetActive(false);
    }

    public void StartNewTurn()
    {
        GameData.current.currentTurn++;
        string newTurnText = "Default Turn Text";

        switch (GameData.current.turnType)
        {
            case TurnType.Player:
                GameData.current.turnType = TurnType.Enemy;
                Debug.Log("start enemy turn");
                newTurnText = "Enemy Turn";
                break;
            case TurnType.Enemy:
                GameData.current.turnType = TurnType.Neutral;
                Debug.Log("start neutral turn");

                newTurnText = "Neutral Turn";
                break;
            case TurnType.Neutral:
                GameData.current.turnType = TurnType.Player;
                Debug.Log("start player turn");

                newTurnText = "Player Turn";
                break;
        }
        StartCoroutine(ShowTurnTextForSeconds(newTurnText));
    }

    private IEnumerator ShowTurnTextForSeconds(string newTurnText)
    {
        turnAnimatorObject.SetActive(true);
        turnText.text = newTurnText;
        yield return new WaitForSeconds(turnTextDuration);
        turnAnimatorObject.SetActive(false);
    }
    
}
