﻿using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    GameManager gameManager;

    [HideInInspector]
    public bool isWarningPopupActive = false;

    [SerializeField]
    GameObject startScreen;

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

    [Header("TURN UI")]
    [SerializeField]
    Text turnText;

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
    GameObject scholarPopup;
    #endregion

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        AddButtListeners();
        ShowLevelUpPopup(new PlayerControls(), false);
        ShowWarningPopup(false);
        DisplayRebirthPopup(false);
        DisplayScholarPopup(false);
        ShowCharPopup(new PlayerControls(), false);

        startScreen.SetActive(true);
        darkVictoryScreen.SetActive(false);
        lightVictoryScreen.SetActive(false);

    }

    private void Start()
    {
        if (!GameData.current.isDebugMode)
            debugText.gameObject.SetActive(false);
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
        //restartGameButton.onClick.AddListener(RestartGame);
    }

    void CharButtonPressed()
    {
        ShowCharPopup(charToShow, true);
    }

    private void LightLevelUp()
    {
        ShowLevelUpPopup(new PlayerControls(), false);
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
            rebirthScreen.SetActive(false);
            return;
        }


        rebirthScreen.SetActive(true);

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
            charPopup.SetActive(false);
            return;
        }
        GameData.current.playerTurnEndTime += 20f;
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
            return;
        }
        playerToLevel = player;
        levelUpPopup.SetActive(true);

        levelUpPicture.sprite = player.bigCharSprite;

        levelUpCharName.text = player.name;

        levelUpTitle.text = "Level " + player.stats.level;

        // STAT INCREASE
        lightStatIncrease = player.stats.GetStatIncreaseAmount(player.stats.lightLevelUpStat);
        darkStatIncrease = player.stats.GetStatIncreaseAmount(player.stats.darkLevelUpStat);

        switch (player.stats.lightLevelUpStat)
        {
            case CharStat.defense:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "defense\n\n+20 lightness";
                levelUp1StatImage.sprite = defenseIcon;
                break;
            case CharStat.speed:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "speed\n\n+20 lightness";
                levelUp1StatImage.sprite = speedIcon;
                break;
            case CharStat.mana:
                levelUpButt1Text.text = lightStatIncrease.ToString();
                levelUpExplanation1.text = "mana\n\n+20 lightness";
                levelUp1StatImage.sprite = manaIcon;
                break;
        }

        switch (player.stats.darkLevelUpStat)
        {
            case CharStat.offense:
                levelUpButt2Text.text = darkStatIncrease.ToString();
                levelUpExplanation2.text = "offense\n\n+20 darkness";
                levelUp2StatImage.sprite = offenseIcon;
                break;
            case CharStat.life:
                levelUpButt2Text.text = darkStatIncrease.ToString();
                levelUpExplanation2.text = "life\n\n+20 darkness";
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

    public void DisplayScholarPopup(bool display = true)
    {
        if (!display)
        {
            scholarPopup.SetActive(false);
            return;
        }
        scholarPopup.SetActive(true);

    }

}
