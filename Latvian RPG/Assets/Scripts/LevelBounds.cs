using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    public int xCoord;
    public int yCoord;
    public Direction direction;

    void Start()
    {
        xCoord = (int)transform.position.x;    
        yCoord = (int)transform.position.y;
        Debug.Log("i'm alive");

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (direction == Direction.Down)
        {
            gameManager.levelBottomBorder = this;
        }
        else if (direction == Direction.Left)
        {
            gameManager.levelLeftBorder = this;
        }
        else if (direction == Direction.Right)
        {
            gameManager.levelRightBorder = this;
        }
        else
        {
            gameManager.levelTopBorder = this;
        }

    }
}
