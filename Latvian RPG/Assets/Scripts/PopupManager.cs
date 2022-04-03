﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    GameManager gameManager;

    [HideInInspector]
    public bool isWarningPopupActive = false;

    [SerializeField]
    Text currFloorText;

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

    // flair
    [SerializeField]
    Text charName;
    [SerializeField]
    Text charBioText;
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
        ShowCharPopup(new PlayerControls(), false);
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
        gameManager.PauseGameAfterSeconds(false, seconds: 1.5f);
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
                playerToLevel.playerSpeed += lightStatIncrease;
                break;
            case CharStat.mana:
                playerToLevel.stats.maxMana += lightStatIncrease;
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
        gameManager.PauseGameAfterSeconds(false, seconds: 1.5f);
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
        if (moonPoints > GameData.current.pointsRequiredPhase2)
        {
            mooningImage.sprite = brightMoon1;
        }
        // brightest
        if (moonPoints > GameData.current.pointsRequiredPhase2)
        {
            mooningImage.sprite = brightMoon2;
        }
    }

    public void ShowCharPopup(PlayerControls playerControls, bool show = true)
    {
        if (!show)
        {
            GameData.current.PauseGame(false);
            charPopup.SetActive(false);
            return;
        }
        GameData.current.PauseGame(true);
        GameData.current.playerTurnEndTime += 5f;
        charPopup.SetActive(true);

        SetMooningLevelHUD();

        /// REPLACE THIS STUFF WITH TEXTMESHJ
        charName.text = playerControls.stats.name;
        charBioText.text = playerControls.stats.bio;

        charPopupImage.sprite = playerControls.bigCharSprite;

        // stats
        expText.text = playerControls.stats.currExp.ToString() + "/" + playerControls.stats.expRequired.ToString() + "xp";

        offenseText.text = "Offense: " + playerControls.stats.offense.ToString();
        armorText.text = "Defense: " + playerControls.stats.defense.ToString();

        lifeText.text = ((int)playerControls.stats.currLife).ToString() + "/" + ((int)playerControls.stats.maxLife).ToString();
        manaText.text = "Mana: " + playerControls.stats.currMana.ToString() + "/" + playerControls.stats.maxMana.ToString();

        speedText.text = "Speed: " + playerControls.stats.speed;

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

        StartCoroutine(gameManager.PauseGameAfterSeconds(true, seconds: 1.5f));
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
                levelUpExplanation2.text = "offense\n\n+" + levelUpStatAmount + "darkness";
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
            GameData.current.PauseGame(false);
            return;
        }
        GameData.current.PauseGame(true);
        scholarPopup.skillToTeach = skillToDisplay;
        scholarPopupObject.SetActive(true);

        scholarPopup.scholarText.text =
            "An old man offers to teach you a new skill. Learn " +
            skillToDisplay.skillName + "?";

        scholarPopup.skillName.text = skillToDisplay.skillName;

        scholarPopup.skillText.text = skillToDisplay.GetDescription();

        scholarPopup.darkSmallText.text = "\n\n+" + GameData.current.levelUpPointsReward + " darkness";
        scholarPopup.lightSmallText.text = "\n\n+" + GameData.current.levelUpPointsReward + " lightness";

    }

    public void ScholarButtPress(bool isLight = true)
    {
        scholarPopupObject.SetActive(false);
        GameData.current.PauseGame(false);

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
        foreach(SkillButton skillButt in skillButts)
        {
            skillButt.gameObject.SetActive(display);

            if (display)
            {
                if (currSkillID < charToDisplay.stats.skills.Count)
                {
                    skillButt.skillButtonText.text = charToDisplay.stats.skills[currSkillID].skillName;
                    skillButt.skill = charToDisplay.stats.skills[currSkillID];
                }
                else
                    skillButt.gameObject.SetActive(false);
            }
            currSkillID++;
        }
    }

    private void InitializeSkillButts(int startingID)
    {
        for (int i = startingID; i < skillButtCount; i++)
        {
            int id = i; // dunno why doesnt work if I dont save it as a separate variable
            skillButts[id].thisButton.onClick.AddListener((delegate { gameManager.SelectSkill(id); }));
        }
    }

    public void ColorSkillButts(Color newColor)
    {
        foreach(SkillButton skillButton in skillButts)
        {
            skillButton.skillButtonImage.color = newColor;
        }
    }

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
