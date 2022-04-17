using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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
[Serializable]
public class PlayerControls : MonoBehaviour
{
    #region MANAGERS
    private GameManager gameManager;
    private SkillManager skillManager;
    #endregion

    #region movement
    public bool isMovingNow = false;
    private Coroutine currMovementCoroutine;
    #endregion
    public bool availableInRoster = false;
    public bool hasActedThisTurn = false;
    public CharType charType;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool hasUsedSkillThisTurn = false;

    public float xCoord;
    public float yCoord;

    public NPC npcController;
    public Character character;
    public CharacterStats stats;
    [SerializeField]
    private int playerSpeed; // how many tiles can char move in single turn
    public bool characterIsSelected;

    [SerializeField]
    private CharacterSelectArea characterSelector;

    #region SKILLS
    [Header("SKILLS")]
    [SerializeField]
    public Skill[] startingSkills;
    public List<Skill> currentSkills = new List<Skill>();
    public List<Skill> scholarOfferedSkills = new List<Skill>();
    #endregion

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
    StatChangeAnim statChangeAnim;
    [SerializeField]
    Animator charTextAnimator;


    [SerializeField]
    Text roundManaText;
    float currManaText;
    [SerializeField]
    Text roundLifeText;
    float currLifeText;
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

    private void Awake()
    {
        currentSkills.Clear();
        UpdateCoordAndSortOrder();
        GetCharData();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        charMarker.UpdateMarkerColor(charType);

        InitializePlayerStatUI();
    }

    private void Start()
    {
        statChangeAnim = charAnimatedText.transform.parent.gameObject.GetComponent<StatChangeAnim>();

        currLifeText = stats.currLife;
        currManaText = stats.currMana;
        StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds: 0.1f, charStat:CharStat.life, animateAll: true));
    }
    public void GetCharData()
    {
        stats = new CharacterStats(character);
        if (charType != CharType.Player)
            UpdateStatsToCurrDungeonFloor();

        if (currentSkills.Count < startingSkills.Length)
        {
            foreach (Skill newSkill in startingSkills)
            {
                stats.skills.Add(newSkill.skillName);
                currentSkills.Add(newSkill);
            }
        }

        name = stats.name;
        playerSpeed = stats.speed;
        characterIsSelected = false;
        defaultSortingOrder = spriteRenderer.sortingOrder;
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

    public bool Convert(CharType targetType)
    {
        if (charType == CharType.Player)
        {
            gameManager.popupManager.UpdateGuideText(this.name + " already is in your party!");
            return false;
        }

        if (targetType == CharType.Player)
        {
            if (TryJoinPlayerParty())
            {
                hasUsedSkillThisTurn = true;
                return true;
            }
            else return false;
        }
        else return false;
    }

    private bool TryJoinPlayerParty()
    {
        int playerCharCount = 0;
        foreach(PlayerControls character in gameManager.allCharacters)
        {
            if (character.charType == CharType.Player)
                playerCharCount++;
        }
        if (playerCharCount > 2)
        {
            gameManager.popupManager.UpdateGuideText("Cannot recruit more characters!");
            return false;
        }

        charType = CharType.Player;
        transform.parent = null;
        gameManager.popupManager.UpdateGuideText(this.name + " joins you!");
        gameManager.audioManager.PlayUtilitySFX();
        
        GameData.current.currMoonPoints += GameData.current.recruitPointsReward;
        charMarker.UpdateMarkerColor(charType);
        return true;
    }

    public void AddMana(float amount, bool addedBySkill, bool removeMana = false, bool addToFull = false, bool animate = true)
    {
        if (isDead)
            return;
        if (addToFull)
            amount = stats.maxMana - stats.currMana;
        if (stats.currMana < stats.maxMana || removeMana)
        {
            // UI
            if (!removeMana)
            {
                if (addedBySkill)
                {
                    GameData.current.currMoonPoints += GameData.current.healPointsReward;
                }
            }
            if (animate)
            {
                if (removeMana && (MathUtils.FastApproximately(stats.currMana + amount,0,0.01f)||
                    stats.currMana + amount < 0))
                {
                    Debug.LogError("heeyey");
                    float animatedAmount = amount - (stats.currMana + amount);
                    AnimateCharStatus(CharStat.mana, animatedAmount);
                }
                else
                    AnimateCharStatus(CharStat.mana, amount);
            }

            if(addedBySkill)
                gameManager.audioManager.PlayUtilitySFX();

            // HUD
            StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds: 1f, charStat: CharStat.mana));
        }

        // ADD MANA
        stats.currMana += amount;
        if (stats.currMana > stats.maxMana)
        {
            stats.currMana = stats.maxMana;
        }
        if (stats.currMana < 0)
            stats.currMana = 0;
        gameManager.popupManager.HideUnusableButts(this);
    }

    public void AnimateCharStatus(CharStat charStat, float amount)
    {
        if (CharStat.mana == charStat)
        {
            charAnimatedText.text = amount + " mana";
            statChangeAnim.statImage.sprite = statChangeAnim.manaSprite;


        }
        else if (charStat == CharStat.life)
        {
            charAnimatedText.text = amount + " life";
            statChangeAnim.statImage.sprite = statChangeAnim.lifeSprite;
        }
        if (amount > 0)
            charAnimatedText.text = "+" + charAnimatedText.text;
        
        charTextAnimator.SetTrigger("appear");
    }

    public void ManagePlayerMovement()
    {
        if (stats.tilesWalked < stats.speed && characterIsSelected)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                MoveCharacterOneTile(Direction.Up);
                stats.ChangeWalkedTiles(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                MoveCharacterOneTile(Direction.Down);
                stats.ChangeWalkedTiles(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveCharacterOneTile(Direction.Left);
                stats.ChangeWalkedTiles(1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveCharacterOneTile(Direction.Right);
                stats.ChangeWalkedTiles(1);
            }
            
        }
    }

    /// <summary>
    /// Make player character spend movement points to go to target location
    /// 
    /// OR
    /// 
    /// instantly move player to location without spending move points
    /// </summary>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    /// <param name="instantTeleport"></param>
    public void TeleportPlayerCharacter (float targetX, float targetY, bool instantTeleport = false)
    {
        if (isMovingNow && !instantTeleport)
        {
            return;
        }
        float currX = transform.position.x;
        float currY = transform.position.y;

        // CALCULATE WALKED TILES
        float xDiff = targetX - currX;
        float yDiff = targetY - currY;

        int walkedTiles = (int) (Mathf.Abs(xDiff) + Mathf.Abs(yDiff));

        if (instantTeleport)
        {
            Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);
            Debug.Log("should teleport to " + targetPos);
            transform.position = targetPos;
            UpdateCoordAndSortOrder();
            gameManager.HideActionRange();
            gameManager.DisplayActionRange(ActionType.Walk);
            if (isMovingNow)
            {
                isMovingNow = false;
                StopCoroutine(currMovementCoroutine);
            }
            return;
        }
        isMovingNow = true;
        Debug.LogError("MOVING CHAR");
        // MOVE CHARACTER
        currMovementCoroutine = StartCoroutine(MovePlayerCloserToTarget(new Vector2(targetX, targetY),walkedTiles));

       
    }

    private bool IsTileFree(float x, float y)
    {
        if (!gameManager.IsTileAllowedForNPC(new Vector2(x, y)))
            return false;
        foreach (PlayerControls character in gameManager.allCharacters)
        {
            if (character.xCoord == x 
                && character.yCoord == y)
                return false;
        }
        return true;
    }

    public bool RandomMoveNPC()
    {
        bool targetPositionFound = false;
        int directionCount = Direction.GetNames(typeof(Direction)).Length - 1;
        Direction direction = (Direction)UnityEngine.Random.Range(0, directionCount);
        Vector3 targetPosition = new Vector3(xCoord, yCoord - 1, 0); ;
        int whileCounter = 5;

        /// WHAT IF SURROUNDED?
        while (!targetPositionFound)
        {
            switch(direction)
            {
                case Direction.Down:
                    targetPosition = new Vector3(xCoord, yCoord-1, 0);
                    break;
                case Direction.Left:
                    targetPosition = new Vector3(xCoord-1, yCoord, 0);
                    break;
                case Direction.Right:
                    targetPosition = new Vector3(xCoord+1, yCoord, 0);
                    break;
                case Direction.Up:
                    targetPosition = new Vector3(xCoord, yCoord+1, 0);
                    break;
            }
            targetPositionFound = IsTileFree(targetPosition.x, targetPosition.y);
            whileCounter--;
            if (whileCounter < 0)
                return false;
        }
        
        MoveCharacterOneTile(direction);
        Debug.LogError("ayeea;jf");
        stats.ChangeWalkedTiles(1);
        gameManager.UpdateRemainingMovesText(stats.speed - stats.tilesWalked);
        return true;
    }



    public void MoveCharacterOneTile(Direction moveDirection)
    {
        gameManager.UpdateRemainingMovesText(stats.speed - stats.tilesWalked);
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
        if (charType == CharType.Player)
        {
            switch (gameManager.CheckInteractableObject(new Vector2(xCoord, yCoord)))
            {
                case ObjectType.HealingPotion:
                    ConsumeHealthPack();
                    break;
                case ObjectType.LevelExit:
                    isMovingNow = true;
                    EnterNextLevel();
                    break;
                case ObjectType.LearnSkill:
                    // code called in gamemanager
                    break;
                case ObjectType.Undefined:
                    break;
            }
        }
    }

    public void EnterNextLevel()
    {
        GameData.current.EnterNextFloor();
        
        // FADE TRANSITION?

        // ANIMATE TEXT - NEW LEVEL REACHED

        // DELETE EXISTING NPC CHARACTERS
        gameManager.RemoveOldNPCs();

        // SPAWN NEW LEVEL PREFAB
        gameManager.SpawnNewFloor();

        // MOVE OTHER PLAYER CHARS TO NEW POS
        // MOVE main PLAYER CHAR TO NEW POSITION
        gameManager.MovePlayerCharsToFloorStart();
    }

    private void ConsumeHealthPack()
    {
        Debug.Log("consuming hp");
        float healAmount = stats.maxLife - stats.currLife;
        TakeDamage(healAmount, damageSource: this);
    }

    public void SelectCharacter(bool select)
    {
        characterIsSelected = select;
        //characterSelector.characterFrame.SetActive(select);
    }

    public void UpdateCoordAndSortOrder()
    {
        xCoord = transform.position.x; 
        yCoord = transform.position.y;

        defaultSortingOrder = (int)(startingSortingOrder - yCoord);
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
            AnimateCharStatus(CharStat.life, amount);
        }
        // HEALING
        else if (amount > 0)
        {
            gameManager.popupManager.UpdateGuideText(damageSource.name + " is healed!");
            gameManager.audioManager.PlayUtilitySFX();
            AnimateCharStatus(CharStat.life, amount);
        }
        stats.currLife += damageDealt;

        Debug.Log(gameObject.name + " takes " + damageDealt + " damage (remaining hp: " + stats.currLife + ")" );


        // COMBAT LOG - taking damage
        if (charType == CharType.Player || charType == CharType.Enemy)
        gameManager.popupManager.UpdateGuideText(
            damageSource.name + " dealt " + -damageDealt + " damage to " + name + "!");

        else
        {
            gameManager.popupManager.UpdateGuideText(
            name + " becomes an enemy!");
            charType = CharType.Enemy;
            charMarker.UpdateMarkerColor(charType);
        }
        // DEATH
        if (stats.currLife <= 0)
        {
            damageSource.GainExp(ExpAction.Kill);
            Die(damageSource);
        }
        else if (!Mathf.Approximately(damageDealt,0f))
        {
            // UPDATE HUD BARS
            StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds: 0.5f, charStat: CharStat.life));
        }

        return damageDealt;
    }

    public void GainExp(ExpAction expAction)
    {
        switch(expAction)
        {
            case ExpAction.Kill:
                stats.currExp += 10 + GameData.current.dungeonFloor;
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

    public void LevelUp()
    {
        stats.expRequired = (int)(2.1f * stats.expRequired);
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
        if (UnityEngine.Random.Range(0, 1f) > 0.7f)
            stats.darkLevelUpStat = CharStat.life;
        else
            stats.darkLevelUpStat = CharStat.offense;


        // choose light stats
        //  mana
        //  defense,
        //  speed
        float lightRNG = UnityEngine.Random.Range(0, 1f);
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
        charType = CharType.Neutral;
        Debug.Log(this.name + " IS DEAD");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets movement points and all that jazz at the start of the turn
    /// </summary>
    public void UpdateNewTurnStats()
    {
        Debug.Log("resetting");
        ActivateStatusEffects();
        stats.ChangeWalkedTiles(reset: true);
        hasUsedSkillThisTurn = false;
    }

    public bool CanCharacterAct()
    {
        if (stats.tilesWalked < stats.speed || !hasUsedSkillThisTurn)
            return true;
        else
            return false;
    }

    public void SpendMana(float amount)
    {
        stats.currMana -= amount;
        StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds: 0.5f, charStat: CharStat.mana));
    }

    //private IEnumerator UpdateManaBarWithDelay()
    //{
    //    StartCoroutine(ShowManaBarForXSeconds(3f));
    //    yield return new WaitForSeconds(0.5f);
    //    manaBar.fillAmount = (stats.currMana * 1f) / stats.maxMana;
    //}

    public Skill GetLongestRangeDamageSkill()
    {
        Skill longestSkill = currentSkills[0];
        
        foreach (Skill skilly in currentSkills)
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
        AddMana(stats.manaRegen, false, animate: false);
    }

    private IEnumerator MovePlayerCloserToTarget(Vector2 target, int distance)
    {
        while (distance > 0)
        {
            yield return new WaitForSeconds(GameData.current.playerMoveDuration);
            // FURTHER ON X AXIS - move closer on X axis
            if (Mathf.Abs(target.x - transform.position.x) >
                Mathf.Abs(target.y - transform.position.y))
            {
                if (target.x > transform.position.x)
                {
                    MoveCharacterOneTile(Direction.Right);
                }
                else if (target.x < transform.position.x)
                {
                    MoveCharacterOneTile(Direction.Left);
                }
            }
            // FURTHER ON Y AXIS - move closer on Y axis
            else
            {
                if (target.y > transform.position.y)
                {
                    MoveCharacterOneTile(Direction.Up);
                }
                else if (target.y < transform.position.y)
                {
                    MoveCharacterOneTile(Direction.Down);
                }
            }
            stats.ChangeWalkedTiles(1);
            distance--;
        }

        yield return null; //new WaitForSeconds(GameData.current.playerMoveDuration*0.8f);
        gameManager.HideActionRange();
        gameManager.DisplayActionRange(ActionType.Walk);
        isMovingNow = false;
    }

    #region STATS HUD
    public IEnumerator AnimateStatNumbersForXSeconds(float xSeconds, CharStat charStat, bool animateAll = false)
    {
        int incrementCount = 90;
        float incrementDuration;
        float incrementSize;
        float currStat = stats.currLife;
        float targetStat = currStat;
        Text statText = roundLifeText;

        if (charStat == CharStat.life && !animateAll)
        {
            currStat = currLifeText;
            targetStat = Mathf.Round(stats.currLife * 10f) * 0.1f; // round to 1 decimal
           
        }
        else if (charStat == CharStat.mana && !animateAll)
        {
            statText = roundManaText;
            currStat = currManaText;
            targetStat = Mathf.Round(stats.currMana * 10f) * 0.1f; // round to 1 decimal
        }
        else if (animateAll)
        {
            StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds, CharStat.mana));
            StartCoroutine(AnimateStatNumbersForXSeconds(xSeconds, CharStat.life));
            yield break;
        }

        
        float statDiff = Mathf.Abs(targetStat - currStat);
        incrementSize = statDiff / incrementCount;
        incrementDuration = xSeconds / incrementCount;
        int currentIncrement = 0;

        while (currentIncrement < incrementCount)
        {
            if (targetStat < currStat)
            {;
                currStat -= incrementSize;
            }
            else
                currStat += incrementSize;
            float displayStat = Mathf.Round(currStat * 10);
            displayStat *= 0.1f;
            string displayText = displayStat.ToString();
            string finalDisplayText = "";
            for (int i = 0; i < displayText.Length && i < 3; i++)
            {
                finalDisplayText += displayText[i];
            }
            statText.text = finalDisplayText;
            yield return new WaitForSeconds(incrementDuration);
            currentIncrement++;
        }
        float targetDisplay = Mathf.Round(stats.currLife * 10f) * 0.1f;
        if (Mathf.Approximately(targetDisplay, 0f))
            targetDisplay = 0.1f;
        if (charStat == CharStat.mana)
            targetDisplay = Mathf.Round(stats.currMana * 10f) * 0.1f;
        statText.text = targetDisplay.ToString();

        if (charStat == CharStat.mana)
            currManaText = stats.currMana;
        else if (charStat == CharStat.life)
            currLifeText = stats.currLife;
        

        yield return new WaitForSeconds(xSeconds);


    }

    /*
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
    private IEnumerator UpdateStatBarWithDelay(bool isLifeBar)
    {
        yield return new WaitForSeconds(0.2f);


        float targetFill = (stats.currLife * 1f) / stats.maxLife;
        float maxStatAmount = stats.maxLife;
        float currStatAmount = stats.currLife;
        Image statBar = lifeBar;
        int safetyCounter = 90;
        float fillSpeed = 0.01f;

        if (!isLifeBar)
        {

            targetFill = (stats.currMana * 1f) / stats.maxMana;
            statBar = manaBar;
            maxStatAmount = stats.maxMana;
            currStatAmount = stats.currMana;
        }

        // regeneration
        if (targetFill > statBar.fillAmount)
            fillSpeed *= -1;


        float diff = Mathf.Abs(Mathf.Abs(targetFill) - Mathf.Abs(statBar.fillAmount));
        fillSpeed *= maxStatAmount * 0.15f * diff;
        while (!MathUtils.FastApproximately(statBar.fillAmount, targetFill, 0.001f))
        {
            // regenration
            if (fillSpeed < 0)
            {
                if (statBar.fillAmount >= targetFill)
                    break;
            }
            // taking damage
            else if (fillSpeed > 0)
            {
                if (statBar.fillAmount <= targetFill)
                    break;
            }


            statBar.fillAmount = statBar.fillAmount - (fillSpeed);
            yield return new WaitForSeconds(0.01f);
            safetyCounter--;
            if (safetyCounter < 0)
                break;
        }
        statBar.fillAmount = (currStatAmount * 1f) / maxStatAmount;
    }

    public void InstantUpdateStatBars()
    {
        lifeBar.fillAmount = (stats.currLife * 1f) / stats.maxLife;
        manaBar.fillAmount = (stats.currMana * 1f) / stats.maxMana;
    }
    */
    #endregion

    private void UpdateStatsToCurrDungeonFloor()
    {
        int statIncrease = GameData.current.dungeonFloor;

        int increaseRoll = UnityEngine.Random.Range(0, (int)3);

        switch (increaseRoll)
        {
            case 0:
                stats.currLife += statIncrease;
                stats.maxLife += statIncrease;
                Debug.Log(name + " increased life");
                break;
            case 1:
                stats.offense += statIncrease;
                Debug.Log(name + " increased offense");
                break;
            case 2:
                stats.defense += statIncrease;
                Debug.Log(name + " increased defense");
                break;
        }
    }

    #region STATUS EFFECTS
    public void ActivateStatusEffects()
    {
        int effectCount = stats.activeStatusEffects.Count;
        Debug.Log("hello " + effectCount);
        for  (int i = 0; i < effectCount && i > -1; i++)
        {
            SkillEffect effect = stats.activeStatusEffects[i];

            // MANA FLAT
            if (effect.manaIncrease != 0)
                AddMana(effect.manaIncrease, true);

            // ARMOR PERCENT
            if (effect.armorPercentIncrease != 0)
            {
                if (!effect.armorIncreased)
                {
                    Debug.Log("increasing armor!");
                    effect.armorIncreased = true;
                    effect.armorIncrease = (int)(stats.defense * (1f + effect.armorPercentIncrease)) - stats.defense;
                    stats.defense += effect.armorIncrease;
                }
            }

            // SPEED FLAT
            if (effect.speedIncrease != 0)
            {
                if (!effect.speedIncreased)
                {
                    Debug.Log("modyfing speed!");
                    effect.speedIncreased = true;
                    effect.finalSpeed = stats.speed + effect.speedIncrease;
                    if (effect.finalSpeed < 0)
                        stats.speed = 0;
                    else
                        stats.speed = effect.finalSpeed;
                }
            }

            effect.effectDuration--;
            if (effect.effectDuration <= 0)
            {
                Debug.Log("effect run out");
                stats.activeStatusEffects.RemoveAt(i);

                // REMOVE ACTIVE EFFECTS
                if (effect.armorIncreased)
                     stats.defense -= effect.armorIncrease;

                if (effect.speedIncreased)
                    stats.speed = effect.finalSpeed - effect.speedIncrease;

                i--;
                effectCount--;
            }

        }
    }

    public string GetStatusEffectDescript()
    {
        string description = "";

        int effectCount = stats.activeStatusEffects.Count;
        Debug.Log("hello " + effectCount);
        for (int i = 0; i < effectCount && i > -1; i++)
        {
            SkillEffect effect = stats.activeStatusEffects[i];

            string currStatusDescr = "";

            currStatusDescr = stats.activeStatusEffects[i].GetSkillEffectDescr();
            description += currStatusDescr;


            if (i < effectCount - 1)
                description += "\n \n";
        }
        if (effectCount <= 0)
        {
            description = "NO ACTIVE STATUS EFFECTS";
        }

        return description;
    }

    #endregion

    #region SKILL MANAGEMENT
    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillToLearn"></param>
    /// <param name="isLoadedSkill">has this skill been learned in a previous save file?</param>
    public void LearnSkill(Skill skillToLearn, bool isLoadedSkill = false)
    {
        if (!isLoadedSkill)
        {
            stats.skills.Add(skillToLearn.skillName);
        }
        currentSkills.Add(skillToLearn);
        
        if (!isLoadedSkill)
            gameManager.popupManager.DisplayCharSkillButts(this);
    }

    public void LoadSavedSkills(SkillManager skillManager)
    {
        foreach (string loadedSkillName in stats.skills)
        {
            bool skillLearned = false;
            foreach (Skill currSkill in currentSkills)
            {
                if (currSkill.skillName == loadedSkillName)
                    skillLearned = true;
            }
            if (!skillLearned)
            {
                LearnSkill(skillManager.GetSkill(loadedSkillName), isLoadedSkill: true);
            }
        }
        gameManager.popupManager.DisplayCharSkillButts(this);
    }

    public string GetAllSkillDescriptions()
    {
        string description = "";


        int skillCount = currentSkills.Count;

        for (int i = 0; i < skillCount; i++)
        {
            string currSkillDescr = "";

            currSkillDescr = currentSkills[i].GetCharPanelDescription();
            description += currSkillDescr;
            if (i < skillCount-1)
                description += "\n \n";
        }

        return description;
    }

    #endregion

}
