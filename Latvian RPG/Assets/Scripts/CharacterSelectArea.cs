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
        spriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
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
        if (GameData.current.isGamePaused)
            return;

        gameManager.HighlightChar(characterController);
        if (characterController.charType != CharType.Player)
        {
            if (GameData.current.turnType != CharType.Player)
            {
                return;
            }
            else
            {
                gameManager.ProcessInteractionRequest(characterController.xCoord, characterController.yCoord, ActionType.Unknown);
                return;
            }
        }
        if (gameManager.skillSelected) 
        {
            Debug.LogError(gameManager.selectedSkill.type[0]);
            Skill selectedSkill = gameManager.selectedSkill;
            if (selectedSkill.type[0] == SkillType.Buff
                && selectedSkill.skillName != "Teleport")
            {
                gameManager.ProcessInteractionRequest(characterController.xCoord, characterController.yCoord, ActionType.UseUtilitySkill);
                return;
            }
        }
        gameManager.SelectChar(characterController);
            //characterFrame.SetActive(characterController.characterIsSelected);
        //characterSelectedCount++;
    }

}
