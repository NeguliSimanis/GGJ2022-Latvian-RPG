using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

/// <summary>
/// Action that gives regular exp and/or moon points
/// </summary>
public enum ExpAction
{
    Kill,
    Hire,
    Heal,
    LevelUpDark,
    LevelUpLight
}

public class PlayerControls : MonoBehaviour
{
    public bool hasActedThisTurn = false;
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
    Image manaBarBG;
    Color manaColor;
    [SerializeField]
    Image lifeBar;
    Color lifeColor;
    Image lifeBarBG;
    [SerializeField]
    Text charAnimatedText;
    [SerializeField]
    Animator charTextAnimator;
    #endregion

    [Header("VISUAL")]
    public CharacterMarker charMarker;
    public Sprite charPortrait;
    public Sprite bigCharSprite;
    public SpriteRenderer spriteRenderer;
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
        charMarker.UpdateMarkerColor(type);

        InitializePlayerStatUI();
    }

    private void InitializePlayerStatUI()
    {
        lifeBarBG = lifeBar.transform.parent.GetChild(0).gameObject.GetComponent<Image>();
        manaBarBG = manaBar.transform.parent.GetChild(0).gameObject.GetComponent<Image>();

        manaColor = manaBar.color;
        lifeColor = lifeBar.color;

        // Fade in character stats
        lifeBarBG.color = new Color(1, 1, 1, 0);
        manaBarBG.color = new Color(1, 1, 1, 0);
        FadeImage(lifeBar, fadeFromTransparent: true, fadeSpeed: 0.01f, 
            startColor: new Color(lifeColor.a, lifeColor.b, lifeColor.g, 0),
            endColor: new Color(lifeColor.a, lifeColor.b, lifeColor.g, 0.3f));
        FadeImage(manaBar, fadeFromTransparent: true, fadeSpeed: 0.02f,
            startColor: new Color(manaColor.a, manaColor.b, manaColor.g, 0),
            endColor: new Color(manaColor.a, manaColor.b, manaColor.g, 0.3f));
        //FadeImage(lifeBarBG, fadeIn: true, fadeSpeed: 0.02f,
        //    startColor: new Color(1,1,1,0),
        //    endColor: new Color(1, 1, 1, 1));
        //FadeImage(manaBarBG, fadeIn: true, fadeSpeed: 0.02f,
        //    startColor: new Color(1, 1, 1, 0),
        //    endColor: new Color(1, 1, 1, 1));
    }

    private void FadeImage(Image imageToFade, float fadeSpeed, Color startColor, Color endColor, bool fadeFromTransparent)
    {
        if (!fadeFromTransparent)
        {
            StartCoroutine(VisualUtils.FadeImage(fadeSpeed, imageToFade, imageToFade.color, new Color(1, 1, 1, 0)));
        }
        else 
        {
            StartCoroutine(VisualUtils.FadeImage(fadeSpeed, imageToFade, 
                startColor: startColor, 
                endColor: endColor));
        }
    }

    public void Convert(CharType targetType)
    {
        if (type == CharType.Player)
        {
            gameManager.popupManager.UpdateGuideText(this.name + " already is in your party!");
            return;
        }
        switch (targetType)
        {
            case CharType.Player:
                type = CharType.Player;
                gameManager.popupManager.UpdateGuideText(this.name + " joins you!");
                gameManager.audioManager.PlayUtilitySFX();
                GameData.current.currMoonPoints += GameData.current.recruitPointsReward;
                charMarker.UpdateMarkerColor(type);
                break;
            default:
                Debug.Log("THIS IS NOT IMPLEMENTED");
                break;
        }
    }

    public void AddMana(float amount, bool addedBySkill)
    {
        if (stats.currMana < stats.maxMana)
        {
            // UI
            charAnimatedText.text = "+" + amount + " mana";
            charTextAnimator.SetTrigger("appear");

            if (addedBySkill)
            {
                GameData.current.currMoonPoints += GameData.current.healPointsReward;
                gameManager.audioManager.PlayUtilitySFX();
            }
            // HUD
            StartCoroutine(ShowManaBarForXSeconds(3f));
            StartCoroutine(UpdateStatBarWithDelay(false));
        }

        // ADD MANA
        stats.currMana += amount;
        if (stats.currMana > stats.maxMana)
        {
            stats.currMana = stats.maxMana;
        }

       
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

    public void TeleportPlayerCharacter (int targetX, int targetY, bool instantTeleport = false)
    {
        int currX = (int)transform.position.x;
        int currY = (int)transform.position.y;

        // CALCULATE WALKED TILES
        int xDiff = targetX - currX;
        int yDiff = targetY - currY;

        int walkedTiles = Mathf.Abs(xDiff) + Mathf.Abs(yDiff);

        if (instantTeleport)
        {
            Debug.Log("should teleport");
            transform.position = new Vector3(targetX, targetY, transform.position.z);   
            UpdateCoordAndSortOrder();
            gameManager.HideActionRange();
            gameManager.DisplayActionRange(ActionType.Walk);
            return;
        }

        // MOVE CHARACTER
        StartCoroutine(MovePlayerCloserToTarget(new Vector2Int(targetX, targetY),walkedTiles));
        
        //transform.position = new Vector3((float)(targetX), (float)targetY, transform.position.z);
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

    public bool RandomMoveNPC()
    {
        bool targetPositionFound = false;
        int directionCount = Direction.GetNames(typeof(Direction)).Length - 1;
        Direction direction = (Direction)Random.Range(0, directionCount);
        Vector3Int targetPosition = new Vector3Int(xCoord, yCoord - 1, 0); ;
        int whileCounter = 5;

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
                return false;
        }
        
        MoveCharacterOneTile(direction);
        tilesWalked++;
        gameManager.UpdateRemainingMovesText(playerSpeed - tilesWalked);
        return true;
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
        gameManager.audioManager.PlayStepSound();
        UpdateCoordAndSortOrder();
        if (type == CharType.Player)
        {
            switch (gameManager.CheckInteractableObject(new Vector2Int(xCoord, yCoord)))
            {
                case ObjectType.HealingPotion:
                    ConsumeHealthPack();
                    break;
                case ObjectType.LevelExit:
                    EnterNextLevel();
                    break;
                case ObjectType.Undefined:
                    break;
            }
        }
    }

    private void EnterNextLevel()
    {
        Debug.Log("SHOULD GO TO NEW LEVEL");
        // FADE TRANSITION?

        // ANIMATE TEXT - NEW LEVEL REACHED

        // DELETE EXISTING NPC CHARACTERS
        gameManager.RemoveOldNPCs();

        // MOVE main PLAYER CHAR TO NEW POSITION

        // SPAWN NEW LEVEL PREFAB

        // MOVE OTHER PLAYER CHARS TO NEW POS
        gameManager.MovePlayerToFloorStartingPoint();

        

        
        
    }

    private void ConsumeHealthPack()
    {
        Debug.Log("consuming hp");
        float healAmount = stats.maxLife -= stats.currLife;
        TakeDamage(healAmount, damageSource: this);
    }

    public void SelectCharacter(bool select)
    {
        characterIsSelected = select;
        //characterSelector.characterFrame.SetActive(select);
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
            gameManager.audioManager.PlayAttackSound();
            hurtAnimator.SetTrigger("hurt");
        }
        // HEALING
        else if (amount > 0)
        {
            gameManager.popupManager.UpdateGuideText(damageSource.name + " is healed!");
            gameManager.audioManager.PlayUtilitySFX();
        }
        stats.currLife += damageDealt;

        Debug.Log(gameObject.name + " takes " + damageDealt + " damage (remaining hp: " + stats.currLife + ")" );


        // COMBAT LOG - taking damage
        if (type == CharType.Player || type == CharType.Enemy)
        gameManager.popupManager.UpdateGuideText(
            damageSource.name + " dealt " + -damageDealt + " damage to " + name + "!");

        else
        {
            gameManager.popupManager.UpdateGuideText(
            name + " becomes an enemy!");
            type = CharType.Enemy;
        }
        // DEATH
        if (stats.currLife <= 0)
        {
            damageSource.GainExp(ExpAction.Kill);
            Die(damageSource);
        }

        // UPDATE HUD BARS
        StartCoroutine(ShowLifeBarForXSeconds(3f));
        StartCoroutine(UpdateStatBarWithDelay(isLifeBar: true));

        return damageDealt;
    }

    public void GainExp(ExpAction expAction)
    {
        switch(expAction)
        {
            case ExpAction.Kill:
                stats.currExp += 10;
                stats.UpdateProgressToGameVictory(ExpAction.Kill);
                break;
            case ExpAction.Hire:
                stats.currExp += 7;
                stats.UpdateProgressToGameVictory(ExpAction.Hire);
                break;
        }
        if (stats.currExp >= stats.expRequired)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        stats.expRequired = (int)(1.1f * stats.expRequired);
        stats.level++;
        GameData.current.playerTurnEndTime += 15f;
        //gameManager.popupManager.UpdateGuideText(name + "reached level " + stats.level + "!");
        gameManager.audioManager.PlayLevelupSFX();
        ChooseLevelUpStats();
        StartCoroutine(ShowLevelUpPopupAfterDelay(0.8f));
    }

    private void ChooseLevelUpStats()
    {
        // choose dark stats
        //  life 0.3
        //  offense 0.7
        if (Random.Range(0, 1f) > 0.7f)
            stats.darkLevelUpStat = CharStat.life;
        else
            stats.darkLevelUpStat = CharStat.offense;


        // choose light stats
        //  mana
        //  defense,
        //  speed
        float lightRNG = Random.Range(0, 1f);
        if (lightRNG < 0.6f)
            stats.lightLevelUpStat = CharStat.defense;
        else if (lightRNG >= 0.6f && lightRNG < 0.99f)
            stats.lightLevelUpStat = CharStat.mana;
        else
            stats.lightLevelUpStat = CharStat.speed;
    }

    private IEnumerator ShowLevelUpPopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameManager.popupManager.ShowLevelUpPopup(this);
    }

    private void Die(PlayerControls murderer)
    {
        gameManager.popupManager.UpdateGuideText(
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

    private IEnumerator UpdateStatBarWithDelay(bool isLifeBar)
    {
        yield return new WaitForSeconds(0.2f);
        float targetFill = (stats.currLife * 1f) / stats.maxLife;
        if (!isLifeBar)
            targetFill = (stats.currMana * 1f) / stats.maxMana;
        int safetyCounter = 1000;
        float fillSpeed = 0.01f;

        if (targetFill > lifeBar.fillAmount && isLifeBar)
            fillSpeed *= -1;
        if (targetFill > manaBar.fillAmount && !isLifeBar)
            fillSpeed *= -1;

        if (isLifeBar)
        {
            while (!MathUtils.FastApproximately(lifeBar.fillAmount, targetFill, 0.001f))
            {
                lifeBar.fillAmount -= fillSpeed;
                yield return new WaitForSeconds(0.01f);
                safetyCounter--;
                if (safetyCounter < 0)
                    break;
            }
            lifeBar.fillAmount = (stats.currLife * 1f) / stats.maxLife;
        }
        else
        {
            Debug.Log("MANA SATGJH " + manaBar.fillAmount + ". target: " + targetFill);
            while (!MathUtils.FastApproximately(manaBar.fillAmount, targetFill, 0.001f))
            {
                

                manaBar.fillAmount -= fillSpeed;
                yield return new WaitForSeconds(0.01f);
                safetyCounter--;
                if (safetyCounter < 0)
                    break;
            }
            manaBar.fillAmount = (stats.currMana * 1f) / stats.maxMana;
        }
    }

    public void SpendMana(float amount)
    {
        stats.currMana -= amount;
        StartCoroutine(ShowManaBarForXSeconds(3f));
        StartCoroutine(UpdateStatBarWithDelay(false));
    }

    //private IEnumerator UpdateManaBarWithDelay()
    //{
    //    StartCoroutine(ShowManaBarForXSeconds(3f));
    //    yield return new WaitForSeconds(0.5f);
    //    manaBar.fillAmount = (stats.currMana * 1f) / stats.maxMana;
    //}

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
                        //Debug.Log("______________________________________________" + skilly.name + " has longer range than " + longestSkill.name);
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
        charMarker.UpdateSortingOrder(spriteRenderer);
    }

    public void RegenMana()
    {
       
        AddMana(stats.manaRegen, false);
       

    }

    private IEnumerator MovePlayerCloserToTarget(Vector2Int target, int distance)
    {
        while (distance > 0)
        {
            yield return new WaitForSeconds(GameData.current.playerMoveDuration);
            // FURTHER ON X AXIS - move closer on X axis
            if (Mathf.Abs(target.x - xCoord) >
                Mathf.Abs(target.y - yCoord))
            {
                if (target.x > xCoord)
                {
                    MoveCharacterOneTile(Direction.Right);
                }
                else if (target.x < xCoord)
                {
                    MoveCharacterOneTile(Direction.Left);
                }
            }
            // FURTHER ON Y AXIS - move closer on Y axis
            else
            {
                if (target.y > yCoord)
                {
                    MoveCharacterOneTile(Direction.Up);
                }
                else if (target.y < yCoord)
                {
                    MoveCharacterOneTile(Direction.Down);
                }
            }
            tilesWalked++;
            distance--;
        }
        Debug.Log("walk over");
        yield return new WaitForSeconds(GameData.current.playerMoveDuration*0.8f);
        gameManager.HideActionRange();
        gameManager.DisplayActionRange(ActionType.Walk);
    }


    private IEnumerator ShowLifeBarForXSeconds(float xSeconds)
    {

        StartCoroutine(VisualUtils.FadeImage(
          fadeSpeed: 0.03f,
          imageToFade: lifeBar,
              startColor: lifeBar.color,
              endColor: lifeColor));


        yield return new WaitForSeconds(xSeconds);


        StartCoroutine(VisualUtils.FadeImage(
           fadeSpeed: 0.04f,
           imageToFade: lifeBar,
               startColor: lifeColor,
               endColor: new Color(lifeColor.r, lifeColor.g, lifeColor.b, 0.3f)));

    }

    private IEnumerator ShowManaBarForXSeconds(float xSeconds)
    {
        StartCoroutine(VisualUtils.FadeImage(
            fadeSpeed: 0.03f, 
            imageToFade: manaBar,
                startColor: manaBar.color,
                endColor: manaColor));

        yield return new WaitForSeconds(xSeconds);

        StartCoroutine(VisualUtils.FadeImage(
            fadeSpeed: 0.04f,
            imageToFade: manaBar,
                startColor: manaColor,
                endColor: new Color(manaColor.r, manaColor.g, manaColor.b, 0.3f)));

    }
}
