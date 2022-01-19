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
        //ManagePlayerMovement();
    }

    private void ManagePlayerMovement()
    {
        if (tilesWalked < playerSpeed && characterIsSelected)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveCharacterOneTile(Direction.Up);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveCharacterOneTile(Direction.Down);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveCharacterOneTile(Direction.Left);
                tilesWalked++;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveCharacterOneTile(Direction.Right);
                tilesWalked++;
            }
            
        }
    }

    public void TeleportCharacter (int targetX, int targetY)
    {
        int currX = (int)transform.position.x;
        int currY = (int)transform.position.y;

        // CALCULATE WALKED TILES
        int xDiff = targetX - currX;
        int yDiff = targetY - currY;

        int walkedTiles = Mathf.Abs(xDiff) + Mathf.Abs(yDiff);
        Debug.Log("walked " + walkedTiles + " TILES ");

        tilesWalked += walkedTiles;

        // MOVE CHARACTER
        transform.position = new Vector3((float)(targetX), (float)targetY, transform.position.z);

        UpdateCoord();
    }

    private void MoveCharacterOneTile(Direction moveDirection)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount">raw damage to be dealt or healed</param>
    /// <returns>actual damage dealt after increases and reductions</returns>
    public float TakeDamage(float amount, PlayerControls damageSource)
    {
        float damageDealt = amount;
        stats.currLife += damageDealt;

        // ANIMATIONS
        hurtAnimator.SetTrigger("hurt");
        Debug.Log(gameObject.name + " takes " + damageDealt + " damage (remaining hp: " + stats.currLife + ")" );

        // COMBAT LOG
        gameManager.UpdateGuideText(
            damageSource.name + " dealt " + -damageDealt + " damage to " + name + "!");

        // DEATH
        if (stats.currLife <= 0)
            Die(damageSource);


        return damageDealt;
    }

    private void Die(PlayerControls murderer)
    {
        gameManager.UpdateGuideText(
            murderer.name + " killed " + name + "!");
        gameObject.SetActive(false);
    }
}
