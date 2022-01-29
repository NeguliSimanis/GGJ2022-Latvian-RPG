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
    public Skill[] startingSkills;

    #region UI
    [Header("HUD")]
    [SerializeField]
    Image manaBar;
    [SerializeField]
    Image lifeBar;
    #endregion

    [Header("VISUAL")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private int defaultSortingOrder;
    private int startingSortingOrder;

    #region ANIMATIONS
    [SerializeField]
    Animator hurtAnimator;
    #endregion

    private void Start()
    {
        UpdateCoordAndSortOrder();
        stats = new CharacterStats(character);
        foreach (Skill newSkill in startingSkills)
        {
            stats.skills.Add(newSkill);
        }
        name = stats.name;
        playerSpeed = stats.speed;
        characterIsSelected = false;
        defaultSortingOrder = spriteRenderer.sortingOrder;
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

        UpdateCoordAndSortOrder();
    }

    private bool IsTileFree(int x, int y)
    {
        if (!gameManager.IsTileAllowedForNPC(new Vector2Int(x, y)))
            return false;
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

    public void MoveCharacterOneTile(Direction moveDirection)
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
        UpdateCoordAndSortOrder();
    }

    public void SelectCharacter(bool select)
    {
        characterIsSelected = select;
        characterSelector.characterFrame.SetActive(select);
    }

    public void UpdateCoordAndSortOrder()
    {
        xCoord = (int)transform.position.x;
        yCoord = (int)transform.position.y;

        defaultSortingOrder = startingSortingOrder - yCoord;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount">raw damage to be dealt or healed</param>
    /// <returns>actual damage dealt after increases and reductions</returns>
    public float TakeDamage(float amount, PlayerControls damageSource)
    {
        float damageDealt = amount;

        // DAMAGING 
        if (amount < 0)
        {
            damageDealt = MathUtils.CalculateDamage(amount, this.stats.defense, damageSource.stats.offense);
        }
        stats.currLife += damageDealt;

        // ANIMATIONS
        hurtAnimator.SetTrigger("hurt");
        Debug.Log(gameObject.name + " takes " + damageDealt + " damage (remaining hp: " + stats.currLife + ")" );

        // COMBAT LOG
        if (type == CharType.Player || type == CharType.Enemy)
        gameManager.UpdateGuideText(
            damageSource.name + " dealt " + -damageDealt + " damage to " + name + "!");

        else
        {
            gameManager.UpdateGuideText(
            damageSource.name + " dealt " + -damageDealt + " damage to " + name + "! " + name + " becomes an enemy!");
            type = CharType.Enemy;
        }
        // DEATH
        if (stats.currLife <= 0)
            Die(damageSource);

        // UPDATE HUD BARS
        StartCoroutine(UpdateLifeBarWithDelay());

        return damageDealt;
    }

    private void Die(PlayerControls murderer)
    {
        gameManager.UpdateGuideText(
            murderer.name + " killed " + name + "!");
        isDead = true;
        Debug.Log(this.name + " IS DEAD");
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
        Debug.Log("LIFEBAR YPDATE " + stats.currLife + " out of " + stats.maxLife + " lidf");
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

    public Skill GetLongestRangeDamageSkill()
    {
        Skill longestSkill = startingSkills[0];
        
        foreach (Skill skilly in startingSkills)
        {
            foreach (SkillType skillType in skilly.type)
            {
                if (skillType == SkillType.Damage)
                {
                    if (skilly.skillRange >= longestSkill.skillRange)
                    {
                        Debug.Log("______________________________________________" + skilly.name + " has longer range than " + longestSkill.name);
                        longestSkill = skilly;
                    }
                }
            }
        }

        return longestSkill;
    }

    public void SetToDefaultSortOrder(bool defaultOrder = true)
    {
        if (defaultOrder)
        {
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }
        else
        {
            spriteRenderer.sortingOrder = defaultSortingOrder + 9;
        }
    }
}
