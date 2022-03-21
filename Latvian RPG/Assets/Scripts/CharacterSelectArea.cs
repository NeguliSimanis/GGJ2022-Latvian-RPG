using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectArea : MonoBehaviour
{
    Color defaultCharColor = Color.white;
    Color highlightColor = Color.blue;
    SpriteRenderer spriteRenderer;
    GameManager gameManager;

    [SerializeField]
    PlayerControls characterController;

    [Header("UI")]
    [SerializeField]
    GameObject selectCharAnimation;
    [SerializeField]
    public CharacterMarker charMarker; // displayed when character selected


    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultCharColor;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        selectCharAnimation.SetActive(false);
    }

    private void OnMouseOver()
    {
        
        selectCharAnimation.SetActive(true);
        gameManager.ProcessShowCharNameRequest(characterController);
    }

    private void OnMouseExit()
    {
        selectCharAnimation.SetActive(false);
    }


    private void OnMouseDown()
    {

        gameManager.HighlightChar(characterController);
        if (characterController.charType != CharType.Player)
            return;
        gameManager.SelectChar(characterController);
            //characterFrame.SetActive(characterController.characterIsSelected);
        //characterSelectedCount++;
    }

}
