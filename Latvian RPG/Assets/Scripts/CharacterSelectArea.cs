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
    public GameObject characterFrame; // displayed when character selected

    int characterSelectedCount = 0; // how many times char has been selected

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultCharColor;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        selectCharAnimation.SetActive(false);
        characterFrame.SetActive(characterController.characterIsSelected);
    }

    private void OnMouseOver()
    {
        selectCharAnimation.SetActive(true);
    }

    private void OnMouseExit()
    {
        selectCharAnimation.SetActive(false);
    }


    private void OnMouseDown()
    {
        if (characterController.isNPC)
            return;
        if (characterSelectedCount != 0 && gameManager.selectedCharacter == characterController)
        {
            gameManager.ToggleCharInfoPanel(true);
        }
        else
        {

            gameManager.SelectCharacter(characterController);
            characterFrame.SetActive(characterController.characterIsSelected);
        }
        characterSelectedCount++;
    }

}
