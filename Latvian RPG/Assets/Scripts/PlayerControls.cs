using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

public class PlayerControls : MonoBehaviour
{
    int playerSpeed = 5; // how many tiles can char move in single turn
    int tilesWalked = 0;

    #region UI
    [SerializeField]
    Text remainingMovesText;
    [SerializeField]
    Button endTurnButton;
    [SerializeField]
    Text currentTurnText;
    #endregion

    private void Start()
    {
        if (GameData.current == null)
            GameData.current = new GameData();

        endTurnButton.onClick.AddListener(EndTurn);
    }

    private void EndTurn()
    {
        GameData.current.currentTurn++;
        tilesWalked = 0;
        currentTurnText.text = "Turn " + GameData.current.currentTurn.ToString();
    }

    private void Update()
    {
        ManagePlayerMovement();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EndTurn();
        }
        
    }

    private void ManagePlayerMovement()
    {
        if (tilesWalked < playerSpeed)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveCharacter(Direction.Up);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveCharacter(Direction.Down);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveCharacter(Direction.Left);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveCharacter(Direction.Right);
                tilesWalked++;
            }
            UpdateRemainingMovesText();
        }
    }

    private void UpdateRemainingMovesText()
    {
        int remainingMoves = playerSpeed - tilesWalked;
        remainingMovesText.text = "Remaining moves: " + remainingMoves.ToString();
    }

    private void MoveCharacter(Direction moveDirection)
    {
        switch (moveDirection)
        {
            case Direction.Up:
                transform.position = new Vector3(transform.position.x, transform.position.y + GameData.current.tileSize, transform.position.z);
                break;
            case Direction.Down:
                transform.position = new Vector3(transform.position.x, transform.position.y - GameData.current.tileSize, transform.position.z);
                break;
            case Direction.Left:
                transform.position = new Vector3(transform.position.x - GameData.current.tileSize, transform.position.y, transform.position.z);
                break;
            case Direction.Right:
                transform.position = new Vector3(transform.position.x + GameData.current.tileSize, transform.position.y, transform.position.z);
                break;
        }

    }
}
