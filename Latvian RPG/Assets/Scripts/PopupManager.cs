using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum CharPopupSection
{
    Bio,
    Skills,
    Status
}

public class PopupManager : MonoBehaviour
{
    GameManager gameManager;

    [HideInInspector]
    public bool isWarningPopupActive = false;

    [SerializeField]
    Text currFloorText;

    
    public Sprite darkButtonGraphic;
    public Sprite lightButtonGraphic;

    #region START SCREEN
    [Header("START SCREEN")]
    public GameObject startScreen;
    [SerializeField]
    Button loadGameButt;
    #endregion

    [SerializeField]
    GameObject popupObject;
    [SerializeField]
    Button confirmEndTurnButton;
    [SerializeField]
    Button cancelEndTurnButton;

    [SerializeField] Text guideText;

    #region CHAR butt
    [Header("Char  PORTRAIT")]
    public Button charButton;
    [SerializeField]
    Image charPortrait;
    PlayerControls charToShow;
    #endregion

    #region DEBUG
    [Header("DEBUG")]
    [SerializeField]
    Text debugText;
    #endregion

    #region CHARACTER POPUP
    [Header("Character popup")]
    [SerializeField]
    GameObject charPopup;
    private PlayerControls charToDisplay;

    // sections
    CharPopupSection activeCharPopupSection = CharPopupSection.Bio;
    [SerializeField]
    Button charPopupBioButt;
    Text bioButtText;
    [SerializeField]
    Button charPopupSkillButt;
    Text skillButtText;
    [SerializeField]
    Button charPopupStatusButt;
    Text statusButtText;

    // flair
    [SerializeField]
    Text charName;
    [SerializeField]
    Text charBigText;
    [SerializeField]
    Image charPopupImage;

    // stats
    [SerializeField]
    Text expText;

    [SerializeField]
    Text offenseText;
    [SerializeField]
    Text armorText;

    [SerializeField]
    Text lifeText;
    [SerializeField]
    Text manaText;

    [SerializeField]
    Text speedText;
    [SerializeField]
    Text levelText;
    #endregion

    #region LEVEL UP POPUP
    PlayerControls playerToLevel;
    int lightStatIncrease;
    int darkStatIncrease;

    [Header("Level up popup")]
    [SerializeField]
    GameObject levelUpPopup;

    [SerializeField]
    Image levelUpPicture;

    [SerializeField]
    Text levelUpCharName;

    [SerializeField]
    Text levelUpTitle;

    [SerializeField]
    Button levelUpButtonLight;
    [SerializeField]
    Image levelUp1StatImage;
    [SerializeField]
    Text levelUpButt1Text;
    [SerializeField]
    Text levelUpExplanation1;

    [SerializeField]
    Button levelUpButtonDark;
    [SerializeField]
    Image levelUp2StatImage;
    [SerializeField]
    Text levelUpButt2Text;
    [SerializeField]
    Text levelUpExplanation2;
    #endregion

    #region GAME OVER 
    [Header("GAME OVER screens")]
    [SerializeField]
    GameObject lightVictoryScreen;
    [SerializeField]
    GameObject darkVictoryScreen;
    #endregion

    #region MOONING LEVEL
    [Header("Mooning level")]
    [SerializeField]
    Text mooningText;
    [SerializeField]
    Image mooningImage;
    [SerializeField]
    Sprite neutralMoon;
    [SerializeField]
    Sprite brightMoon1;
    [SerializeField]
    Sprite brightMoon2;
    [SerializeField]
    Sprite brightMoon3;
    [SerializeField]
    Sprite darkMoon1;
    [SerializeField]
    Sprite darkMoon2;
    #endregion


    #region STAT ICONS
    [Header("STAT ICONS")]
    [SerializeField]
    public Sprite speedIcon;
    public Sprite lifeIcon;
    public Sprite manaIcon;
    public Sprite defenseIcon;
    public Sprite offenseIcon;
    public Sprite xpIcon;
    #endregion

    #region TURN TEXT
    [Header("TURN UI")]
    [SerializeField]
    Text turnText;
    #endregion

    #region SKILL BUTTS
    [Header("SKILL BUTTS")]
    [SerializeField]
    Transform skillButtPanel;

    [SerializeField]
    GameObject skillButtObj;

    [HideInInspector]
    public List<SkillButton> skillButts = new List<SkillButton>();
    [HideInInspector]
    public int skillButtCount = 0;
    #endregion

    #region REBIRTH
    [Header("REBIRTH")]
    [SerializeField]
    GameObject rebirthScreen;
    [SerializeField]
    Transform rebirthButtPanel;
    [SerializeField]
    GameObject rebirthButtonObj;
    [SerializeField]
    GameObject rebirthOrText;
    [SerializeField]
    Text rebirthFlairText;

    [SerializeField]
    GameObject showRebirthBonusPanel;
    [SerializeField]
    GameObject hideRebirthBonusPanel;
    #endregion

    #region SCHOLAR
    [Header("SCHOLAR")]
    [SerializeField]
    GameObject scholarPopupObject;
    ScholarPopup scholarPopup;
    #endregion

    #region PAUSE MENU
    [Header("PAUSE MENU")]
    [SerializeField]
    GameObject pausePanel;

    [SerializeField]
    Button endPausePanelButt;

    [SerializeField]
    Button saveAndExitButt;
    #endregion

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        AddButtListeners();

            

        ShowLevelUpPopup(new PlayerControls(), false);
        ShowWarningPopup(false);
        DisplayRebirthPopup(false);
        InitializeScholarInfo();
        InitializeCharPopup();
        ShowPausePanel(false);

        startScreen.SetActive(true);
        darkVictoryScreen.SetActive(false);
        lightVictoryScreen.SetActive(false);

    }

    private void InitializeScholarInfo()
    {
        scholarPopup = scholarPopupObject.GetComponent<ScholarPopup>();
        DisplayScholarPopup(display: false);

        scholarPopup.lightOptionButton.onClick.AddListener(delegate { ScholarButtPress(isLight: true); });
        scholarPopup.darkOptionButton.onClick.AddListener(delegate { ScholarButtPress(isLight: false); });
    }

    private void Start()
    {
        if (!GameData.current.isDebugMode)
            debugText.gameObject.SetActive(false);
        UpdateFloorText();
        if (!gameManager.saveManager.SaveAvailable())
            loadGameButt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!GameData.current.isDebugMode)
            return;
        debugText.gameObject.SetActive(true);
        if (gameManager.selectedChar == null)
        {
            debugText.text = "no selected char";
        }
        else
        {
            PlayerControls selectedChar = gameManager.selectedChar;
            debugText.text = selectedChar.name + " - " + selectedChar.stats.currLife + "/" + selectedChar.stats.maxLife + " laif";
        }
    }

    private void AddButtListeners()
    {
        levelUpButtonLight.onClick.AddListener(LightLevelUp);
        levelUpButtonDark.onClick.AddListener(DarkLevelUp);
        confirmEndTurnButton.onClick.AddListener(ConfirmEndTurn);
        cancelEndTurnButton.onClick.AddListener(CancelEndTurn);
        charButton.onClick.AddListener(CharButtonPressed);
        endPausePanelButt.onClick.AddListener((delegate {ShowPausePanel(false);}));
        saveAndExitButt.onClick.AddListener((delegate {gameManager.SaveAndExitToMenu();}));
        loadGameButt.onClick.AddListener((delegate {gameManager.LoadGame();}));
    }

    void CharButtonPressed()
    {
        ShowCharPopup(charToShow, true);
    }

    /// <summary>
    /// happens on selecting light stat button on level up
    /// </summary>
    private void LightLevelUp()
    {
        ShowLevelUpPopup(new PlayerControls(), false);
        gameManager.PauseGameAfterSeconds(false, seconds: 0.5f);
        if (playerToLevel.stats.UpdateProgressToGameVictory(ExpAction.LevelUpLight))
        {
            gameManager.Victory();
        }
        switch (playerToLevel.stats.lightLevelUpStat)
        {
            case CharStat.defense:
                playerToLevel.stats.defense += lightStatIncrease;
                break;
            case CharStat.speed:
                playerToLevel.stats.speed += lightStatIncrease;
                break;
            case CharStat.mana:
                playerToLevel.stats.maxMana += lightStatIncrease;
                StartCoroutine(playerToLevel.AnimateStatNumbersForXSeconds(0.9f, CharStat.life));
                break;
        }

    }

    public void UpdateCharButton(PlayerControls playerControls)
    {
        charPortrait.sprite = playerControls.charPortrait;
        charToShow = playerControls;
    }

    private void DarkLevelUp()
    {
        ShowLevelUpPopup(new PlayerControls(), false);
        gameManager.PauseGameAfterSeconds(false, seconds: 0.5f);
        if (playerToLevel.stats.UpdateProgressToGameVictory(ExpAction.LevelUpDark))
        {
            gameManager.Victory();
        }
        switch (playerToLevel.stats.darkLevelUpStat)
        {
            case CharStat.offense:
                playerToLevel.stats.offense += darkStatIncrease;
                break;
            case CharStat.life:
                playerToLevel.stats.maxLife += darkStatIncrease;
                playerToLevel.stats.currLife += darkStatIncrease;
                StartCoroutine(playerToLevel.AnimateStatNumbersForXSeconds(0.9f, CharStat.life));
                break;
        }
    }

    public void ShowWarningPopup(bool show)
    {
        isWarningPopupActive = show;
        popupObject.SetActive(show);
    }

    private void ConfirmEndTurn()
    {
        gameManager.EndTurn();
    }

    private void CancelEndTurn()
    {
        ShowWarningPopup(false);
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
    }

    public void DisplayRebirthPopup(bool show = true)
    {
        if (!show)
        {
            GameData.current.PauseGame(false);
            rebirthScreen.SetActive(false);
            return;
        }


        rebirthScreen.SetActive(true);
        GameData.current.PauseGame(true);

        int realFloorReached = GameData.current.dungeonFloor + 2;
        int maxFloor = realFloorReached;
        if (GameData.maxFloorReached > maxFloor)
            maxFloor = GameData.maxFloorReached;

        if (realFloorReached < maxFloor)
        {
            showRebirthBonusPanel.SetActive(false);
            hideRebirthBonusPanel.SetActive(true);
            return;
        }
        showRebirthBonusPanel.SetActive(true);
        hideRebirthBonusPanel.SetActive(false);

        int buttCount = CountRebirthButts();
        Debug.Log("REBIRTH BUTT COUNT " + buttCount);
        if (buttCount < 2)
            rebirthOrText.gameObject.SetActive(false);
        for (int i = 0; i < buttCount; i++)
        {
            GameObject newRebirthButtObj = Instantiate(rebirthButtonObj, rebirthButtPanel);
            RebirthStatButton newRebirthButt = newRebirthButtObj.GetComponent<RebirthStatButton>();
            RebirthBonus newRebirthBonus = RebirthManager.instance.GetRebirthBonusInfo();
            newRebirthButt.DisplayStats(newRebirthBonus, this);
        }

        rebirthFlairText.text = "Reached Floor " + realFloorReached + "\n" + "Current best: " + maxFloor;
    }

    private int CountRebirthButts()
    {
        int rebirthButtCount = 0;
        foreach (RebirthBonus rebirthBonus in RebirthManager.instance.rebirthBonuses)
        {
            if (rebirthBonus.amount > 0)
            {
                rebirthButtCount++;
            }
        }
        if (rebirthButtCount > 2)
            rebirthButtCount = 2;
        return rebirthButtCount;
    }

    public void UpdateGuideText(string newText)
    {
        guideText.text = newText;
    }

    public void UpdateTurnText()
    {
        string prefix = "";
        switch (GameData.current.turnType)
        {
            case CharType.Enemy:
                prefix = "enemy turn";
                break;
            case CharType.Player:
                prefix = "player turn";
                break;
            case CharType.Neutral:
                prefix = "neutral turn";
                break;
        }
        turnText.text = prefix;
    }

    private void SetMooningLevelHUD()
    {
        int moonPoints = GameData.current.currMoonPoints;
        mooningText.text = Mathf.Abs(moonPoints).ToString() + "/" + GameData.current.pointsRequiredPhase3;

        // darkest
        if (moonPoints < -GameData.current.pointsRequiredPhase2)
        {
            mooningImage.sprite = darkMoon2;
        }
        // dark
        else if (moonPoints < -GameData.current.pointsRequiredPhase1)
        {
            mooningImage.sprite = darkMoon1;
        }
        // neutral
        else if (moonPoints == 0)
        {
            mooningImage.sprite = neutralMoon;
        }
        // bright
        if (moonPoints > GameData.current.pointsRequiredPhase1)
        {
            mooningImage.sprite = brightMoon1;
        }
        // brightest
        if (moonPoints > GameData.current.pointsRequiredPhase2)
        {
            mooningImage.sprite = brightMoon2;
        }
        if (moonPoints > GameData.current.pointsRequiredPhase25)
        {
            mooningImage.sprite = brightMoon3;
        }
    }

    #region CHARACTER POPUP
    public void ShowCharPopup(PlayerControls playerControls, bool show = true, bool intializePopup = false)
    {
        if (!show)
        {
            GameData.current.PauseGame(false);
            charPopup.SetActive(false);
            return;
        }
        charToDisplay = playerControls;
        charPopup.SetActive(true);

        if (intializePopup)
        {
            bioButtText = charPopupBioButt.transform.GetChild(0).gameObject.GetComponent<Text>();
            skillButtText = charPopupSkillButt.transform.GetChild(0).gameObject.GetComponent<Text>();
            statusButtText = charPopupStatusButt.transform.GetChild(0).gameObject.GetComponent<Text>();
            return;
        }

        GameData.current.PauseGame(true);
        GameData.current.playerTurnEndTime += 5f;
        

        SetMooningLevelHUD();

        
        charName.text = playerControls.stats.name;

        DisplayBigCharPopupText(playerControls);
        

        charPopupImage.sprite = playerControls.bigCharSprite;

        // stats
        expText.text = playerControls.stats.currExp.ToString() + "/" + playerControls.stats.expRequired.ToString() + "xp";

        offenseText.text = "Offense: " + playerControls.stats.offense.ToString();
        armorText.text = "Defense: " + playerControls.stats.defense.ToString();

        float currlife = Mathf.Round(playerControls.stats.currLife * 10f) * 0.1f;
        if (Mathf.Approximately(currlife, 0f))
            currlife = 0.1f;
        lifeText.text = (currlife.ToString() + "/" + ((int)playerControls.stats.maxLife).ToString());
        manaText.text = "Mana: " + playerControls.stats.currMana.ToString() + "/" + playerControls.stats.maxMana.ToString();

        int remainingSpeed = playerControls.stats.speed - playerControls.stats.tilesWalked;
        if (remainingSpeed < 0)
            remainingSpeed = 0;
        speedText.text = "Speed: " + remainingSpeed + "/" + playerControls.stats.speed;

        switch (playerControls.charType)
        {
            case CharType.Enemy:
                levelText.text = "Level " + playerControls.stats.level + " Enemy";
                break;
            case CharType.Player:
                levelText.text = "Level " + playerControls.stats.level;
                break;
            case CharType.Neutral:
                levelText.text = "Level " + playerControls.stats.level + " Neutral";
                break;
        }

    }

    private void DisplayBigCharPopupText(PlayerControls playerControls)
    {
        switch(activeCharPopupSection)
        {
            case CharPopupSection.Bio:
                charBigText.text = playerControls.stats.bio;
                charPopupBioButt.image.sprite = lightButtonGraphic;
                bioButtText.color = Color.black;
                charPopupSkillButt.image.sprite = darkButtonGraphic;
                skillButtText.color = Color.white;
                charPopupStatusButt.image.sprite = darkButtonGraphic;
                statusButtText.color = Color.white;
                break;
            case CharPopupSection.Skills:
                charBigText.text = playerControls.GetAllSkillDescriptions();
                charPopupBioButt.image.sprite = darkButtonGraphic;
                bioButtText.color = Color.white;
                charPopupSkillButt.image.sprite = lightButtonGraphic;
                skillButtText.color = Color.black;
                charPopupStatusButt.image.sprite = darkButtonGraphic;
                statusButtText.color = Color.white;
                break;
            case CharPopupSection.Status:
                charBigText.text = playerControls.GetStatusEffectDescript();
                charPopupBioButt.image.sprite = darkButtonGraphic;
                bioButtText.color = Color.white;
                charPopupSkillButt.image.sprite = darkButtonGraphic;
                skillButtText.color = Color.white;
                charPopupStatusButt.image.sprite = lightButtonGraphic;
                statusButtText.color = Color.black;
                break;
        }
    }


    private void InitializeCharPopup()
    {
        ShowCharPopup(new PlayerControls(), true, true);
        charPopupBioButt.onClick.AddListener((delegate { UpdateBigCharText(CharPopupSection.Bio); }));
        charPopupSkillButt.onClick.AddListener((delegate { UpdateBigCharText(CharPopupSection.Skills); }));
        charPopupStatusButt.onClick.AddListener((delegate { UpdateBigCharText(CharPopupSection.Status); }));
        ShowCharPopup(new PlayerControls(), false);
    }

    public void UpdateBigCharText(CharPopupSection charPopupSection)
    {
        activeCharPopupSection = charPopupSection;
        DisplayBigCharPopupText(gameManager.highlightedChar);
        gameManager.audioManager.PlayButtonSFX();
    }

    #endregion

    public void ShowLevelUpPopup(PlayerControls player, bool show = true)
    {
        if (player.charType != CharType.Player)
            return;
        if (!show)
        {
            levelUpPopup.SetActive(false);
            GameData.current.PauseGame(false);
            return;
        }
        playerToLevel = player;
        levelUpPopup.SetActive(true);

        StartCoroutine(gameManager.PauseGameAfterSeconds(true, seconds: 0.5f));
        GameData.current.playerTurnEndTime += 3f;


        levelUpPicture.sprite = player.bigCharSprite;

        levelUpCharName.text = player.name;

        levelUpTitle.text = "Level " + player.stats.level;

        // STAT INCREASE
        string levelUpStatAmount = GameData.current.levelUpPointsReward.ToString();
        lightStatIncrease = player.stats.GetStatIncreaseAmount(player.stats.lightLevelUpStat);
        darkStatIncrease = player.stats.GetStatIncreaseAmount(player.stats.darkLevelUpStat);

        switch (player.stats.lightLevelUpStat)
        {
            case CharStat.defense:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "defense\n\n+"+levelUpStatAmount +" lightness";
                levelUp1StatImage.sprite = defenseIcon;
                break;
            case CharStat.speed:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "speed\n\n+" + levelUpStatAmount + " lightness";
                levelUp1StatImage.sprite = speedIcon;
                break;
            case CharStat.mana:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "mana\n\n+" + levelUpStatAmount + " lightness";
                levelUp1StatImage.sprite = manaIcon;
                break;
        }

        switch (player.stats.darkLevelUpStat)
        {
            case CharStat.offense:
                levelUpButt2Text.text = darkStatIncrease.ToString();
                levelUpExplanation2.text = "offense\n\n+" + levelUpStatAmount + " darkness";
                levelUp2StatImage.sprite = offenseIcon;
                break;
            case CharStat.life:
                levelUpButt2Text.text = darkStatIncrease.ToString();
                levelUpExplanation2.text = "life\n\n+" + levelUpStatAmount + " darkness";
                levelUp2StatImage.sprite = lifeIcon;
                break;
        }
    }
    public void ShowLightVictory()
    {
        lightVictoryScreen.SetActive(true);
    }

    public void ShowDarkVictory()
    {
        darkVictoryScreen.SetActive(true);
    }

    public void DisplayScholarPopup(Skill skillToDisplay = null, bool display = true)
    {
        if (!display)
        {
            scholarPopupObject.SetActive(false);
            gameManager.PauseGame(false);
            return;
        }
        StartCoroutine(gameManager.PauseGameAfterSeconds(true, seconds: 0.4f, debug: true));
        
        scholarPopup.skillToTeach = skillToDisplay;
        scholarPopupObject.SetActive(true);

        scholarPopup.scholarText.text =
            "An old man offers to teach you a new skill. Learn " +
            skillToDisplay.skillName + "?";

        scholarPopup.lightSkillName.text = skillToDisplay.skillName;

        scholarPopup.lightSkillText.text = skillToDisplay.GetDescription();

        scholarPopup.darkSmallText.text = "\n\n+" + GameData.current.levelUpPointsReward + " darkness";
        scholarPopup.lightSmallText.text = "\n\n+" + GameData.current.levelUpPointsReward + " lightness";

    }

    public void ScholarButtPress(bool isLight = true)
    {
        scholarPopupObject.SetActive(false);

        gameManager.PauseGame(false);

        if (isLight)
        {
            gameManager.selectedChar.LearnSkill(scholarPopup.skillToTeach);
            gameManager.VictoryCheck(ExpAction.LevelUpLight);
        }
        else
        {
            gameManager.VictoryCheck(ExpAction.LevelUpDark);
        }
    }

    #region SKILL BUTTONS
    public void DisplayCharSkillButts(PlayerControls charToDisplay, bool display = true)
    {
        int skillCount = charToDisplay.stats.skills.Count;
        int lastSkillCount = skillButtCount;
       

        while (skillButtCount < skillCount)
        {
            GameObject newSkillButtObj = Instantiate(skillButtObj, skillButtPanel);
            skillButts.Add(newSkillButtObj.GetComponent<SkillButton>());
            skillButtCount++;
        }

        if (lastSkillCount < skillButtCount)
            InitializeSkillButts(lastSkillCount);
        int currSkillID = 0;
        foreach (SkillButton skillButt in skillButts)
        {
            skillButt.gameObject.SetActive(display);
            if (display)
            {

                if (currSkillID < skillCount)
                {

                    skillButt.skillButtonText.text = charToDisplay.currentSkills[currSkillID].skillName;
                    
                    skillButt.skill = charToDisplay.currentSkills[currSkillID];
                   
                }
                else
                    skillButt.gameObject.SetActive(false);
            }
            currSkillID++;
        }
        HideUnusableButts(charToDisplay);
    }

    private void InitializeSkillButts(int startingID)
    {
        for (int i = startingID; i < skillButtCount; i++)
        {
            int id = i; // dunno why doesnt work if I dont save it as a separate variable
            skillButts[id].thisButton.onClick.AddListener((delegate { gameManager.SelectSkill(id); }));
        }
    }

    public void ColorSkillButts(SkillButton thisButton, bool isSelected = false, bool colorAll = true)
    {
        if (isSelected)
        {
            thisButton.skillButtonImage.sprite = lightButtonGraphic;
            thisButton.skillButtonText.color = Color.black;
        }

        if (!colorAll)
            return;
        foreach(SkillButton skillButton in skillButts)
        {
            skillButton.skillButtonImage.sprite = darkButtonGraphic;
            skillButton.skillButtonText.color = Color.white;
        }
    }

    public void HideUnusableButts(PlayerControls charToDisplay, bool hideAll = false)
    {
        foreach (SkillButton skillButt in skillButts)
        {
            if ((charToDisplay.stats.currMana < skillButt.skill.manaCost) || hideAll)
            {
                skillButt.skillButtonImage.color = new Color(1, 1, 1, 0.1f);
                skillButt.skillButtonText.color = new Color(1, 1, 1, 0.1f);
            }
            else
            {
                skillButt.skillButtonImage.color = Color.white;
                skillButt.skillButtonText.color = Color.white;
            }

        }
    }
    #endregion

    public void UpdateFloorText()
    {
        int realFloor = GameData.current.RealFloor();
        currFloorText.text = "FLOOR " + realFloor.ToString();
    }

    public void ShowPausePanel(bool show)
    {
        GameData.current.PauseGame(show);
        gameManager.audioManager.PlayButtonSFX();
        pausePanel.SetActive(show);
    }
}
