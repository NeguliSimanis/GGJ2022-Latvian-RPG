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
    public bool isNPC;

    public int xCoord;
    public int yCoord;

    [SerializeField]
    private Character character;
    public CharacterStats stats;
    public int playerSpeed; // how many tiles can char move in single turn
    public int tilesWalked = 0;
    public bool characterIsSelected;

    [SerializeField]
    private CharacterSelectArea characterSelector;
    GameManager gameManager;
    [SerializeField]
    private Skill[] startingSkills;


    #region ANIMATIONS
    [SerializeField]
    Animator hurtAnimator;
    #endregion

    private void Start()
    {
        UpdateCoord();
        stats = new CharacterStats(character);
        foreach (Skill newSkill in startingSkills)
        {
            stats.skills.Add(newSkill);
        }

        characterIsSelected = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (isNPC)
            return;
        ManagePlayerMovement();
    }

    private void ManagePlayerMovement()
    {
        if (tilesWalked < playerSpeed && characterIsSelected)
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
            
        }
    }

    private void MoveCharacter(Direction moveDirection)
    {
        gameManager.UpdateRemainingMovesText(playerSpeed - tilesWalked);
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
        UpdateCoord();
    }

    public void SelectCharacter(bool select)
    {
        characterIsSelected = select;
        characterSelector.characterFrame.SetActive(select);
    }

    public void UpdateCoord()
    {
        xCoord = (int)transform.position.x;
        yCoord = (int)transform.position.y;
    }

    public void TakeDamage(float amount)
    {
        stats.currLife += amount;
        hurtAnimator.SetTrigger("hurt");
        Debug.Log(gameObject.name + " takes " + amount + " damage (remaining hp: " + stats.currLife + ")" );
        if (stats.currLife <= 0)
            Die();
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
