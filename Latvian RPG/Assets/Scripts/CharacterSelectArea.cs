using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectArea : MonoBehaviour
{
    Color defaultCharColor = Color.white;
    Color highlightColor = Color.blue;
    SpriteRenderer spriteRenderer;
    GameManager gameManager;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = defaultCharColor;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseOver()
    {
        spriteRenderer.color = highlightColor;
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = defaultCharColor;
    }


    private void OnMouseDown()
    {
        gameManager.ToggleCharInfoPanel(true);
    }

}
