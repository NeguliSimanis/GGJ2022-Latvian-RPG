using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    public bool active;
    public bool allowInteraction;
    public ActionType tileActionType;

    private Color transparent = new Color(1, 1, 1, 0);
    
    // attack colors
    [SerializeField]
    private Color attackHighlightColor;
    [SerializeField]
    private Color attackDefaultColor;

    // move colors
    [SerializeField]
    private Color moveHighlightColor;
    [SerializeField]
    private Color moveDefaultColor;

    // interact color
    [SerializeField]
    private Color interactHighlightColor;
    [SerializeField]
    private Color interactDefaultColor;



    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject marker;
    private GameManager gameManager;

    public int xCoord;
    public int yCoord;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        marker.SetActive(false);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UpdateCoord();
    }
    private void OnMouseOver()
    {
        if (active)
        {
            UpdateCoord();
            MarkTile(true);
        }
    }

    private void UpdateCoord()
    {
        xCoord = (int)transform.position.x;
        yCoord = (int)transform.position.y;
    }

    private void OnMouseExit()
    {
        if (active)
        {
            MarkTile(false);
        }
    }


    private void OnMouseDown()
    {
        if (active && allowInteraction)
        {
            gameManager.ProcessInteractionRequest(xCoord, yCoord, tileActionType);
        }
        else if (!allowInteraction)
        {
            gameManager.HighlightChar(gameManager.selectedChar);
        }
    }


    /// <summary>
    /// Make the tile more highlighted when it's under mouse cursor
    /// </summary>
    /// <param name="mark"></param>
    private void MarkTile(bool mark)
    {
        marker.SetActive(mark);
        if (!allowInteraction)
            return;
        switch (tileActionType)
        {
            case ActionType.UseCombatSkill:
                if (mark)
                    spriteRenderer.color = attackHighlightColor;
                else
                    spriteRenderer.color = attackDefaultColor;
                break;
            case ActionType.Walk:
                if (mark)
                    spriteRenderer.color = moveHighlightColor;
                else
                    spriteRenderer.color = moveDefaultColor;
                break;
            default:
                if (mark)
                    spriteRenderer.color = interactHighlightColor;
                else
                    spriteRenderer.color = interactDefaultColor;
                break;
        }
    }

    public void EnableTile(ActionType actionType, bool show = true, bool allowInteracting = true)
    {
        allowInteraction = allowInteracting;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        active = show;
        if (!show)
        {
            marker.SetActive(false);
            spriteRenderer.color = transparent;
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        tileActionType = actionType;
        switch (tileActionType)
        {
            case ActionType.Walk:
                spriteRenderer.color = moveDefaultColor;
                break;
            case ActionType.UseCombatSkill:
                Debug.Log("use combat skill");
                spriteRenderer.color = attackDefaultColor;
                break;
            default:
                spriteRenderer.color = interactDefaultColor;
                break;
        }
    }
}
