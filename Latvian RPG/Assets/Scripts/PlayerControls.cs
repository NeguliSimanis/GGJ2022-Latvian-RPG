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

public enum CharType
{
    Player,
    Enemy,
    Neutral
}

public class PlayerControls : MonoBehaviour
{
    
    public CharType type;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool hasUsedSkillThisTurn = false;

    public int xCoord;
    public int yCoord;

    public NPC npcController;
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

    #region UI
    [Header("HUD")]
    [SerializeField]
    Image manaBar;
    [SerializeField]
    Image lifeBar;
    #endregion

    private void Start()
    {
        UpdateCoord();
        stats = new CharacterStats(character);
        foreach (Skill newSkill in startingSkills)
        {
            stats.skills.Add(newSkill);
        }
        name = stats.name;
        playerSpeed = stats.speed;
        characterIsSelected = false;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }


    public void ManagePlayerMovement()
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

    private bool IsTileFree(int x, int y)
    {
        foreach (PlayerControls character in gameManager.allCharacters)
        {
            if (character.xCoord == x && character.yCoord == y)
                return false;
        }
        return true;
    }

    public void RandomMoveNPC()
    {
        bool targetPositionFound = false;
        int directionCount = Direction.GetNames(typeof(Direction)).Length - 1;
        Direction direction = (Direction)Random.Range(0, directionCount);
        Vector3Int targetPosition = new Vector3Int(xCoord, yCoord - 1, 0); ;
        int whileCounter = 10;

        /// WHAT IF SURROUNDED?
        while (!targetPositionFound)
        {
            Debug.Log("checking pos");
            switch(direction)
            {
                case Direction.Down:
                    targetPosition = new Vector3Int(xCoord, yCoord-1, 0);
                    break;
                case Direction.Left:
                    targetPosition = new Vector3Int(xCoord-1, yCoord, 0);
                    break;
                case Direction.Right:
                    targetPosition = new Vector3Int(xCoord+1, yCoord, 0);
                    break;
                case Direction.Up:
                    targetPosition = new Vector3Int(xCoord, yCoord+1, 0);
                    break;
            }
            targetPositionFound = IsTileFree(targetPosition.x, targetPosition.y);
            whileCounter--;
            if (whileCounter < 0)
                return;
        }
        
        MoveCharacterOneTile(direction);
        tilesWalked++;
        gameManager.UpdateRemainingMovesText(playerSpeed - tilesWalked);
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

        // UPDATE HUD BARS
        StartCoroutine(UpdateLifeBarWithDelay());

        // DEATH
        if (stats.currLife <= 0)
            Die(damageSource);


        return damageDealt;
    }

    private void Die(PlayerControls murderer)
    {
        gameManager.UpdateGuideText(
            murderer.name + " killed " + name + "!");
        isDead = true;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets movement points and all that jazz at the start of the turn
    /// </summary>
    public void UpdateNewTurnStats()
    {
        tilesWalked = 0;
        hasUsedSkillThisTurn = false;
    }

    public bool CanCharacterAct()
    {
        if (tilesWalked < playerSpeed || !hasUsedSkillThisTurn)
            return true;
        else
            return false;
    }

    private IEnumerator UpdateLifeBarWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        lifeBar.fillAmount = (stats.currLife * 1f) / stats.maxLife;
    }

    public void SpendMana(float amount)
    {
        stats.currMana -= amount;
        StartCoroutine(UpdateManaBarWithDelay());
    }

    private IEnumerator UpdateManaBarWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        manaBar.fillAmount = (stats.currMana * 1f) / stats.maxMana;
    }
}
