using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    public bool active;
    
    
    
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

    int xCoord;
    int yCoord;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        attackDefaultColor = spriteRenderer.color;
        marker.SetActive(false);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        UpdateCoord();
    }
    private void OnMouseOver()
    {
        if (active)
        {
            UpdateCoord();
            spriteRenderer.color = attackHighlightColor;
            marker.SetActive(true);
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
            spriteRenderer.color = attackDefaultColor;
            marker.SetActive(false);
        }
    }


    private void OnMouseDown()
    {
        Debug.Log("test");
        gameManager.ProcessSkillUseRequest(xCoord,yCoord);
    }

    public void ShowTile(bool show = true)
    {
        active = show;
        if (!show)
        {
            marker.SetActive(false);
        }
    }
}
