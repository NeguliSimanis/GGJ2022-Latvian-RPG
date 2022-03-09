using UnityEngine;
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

    [SerializeField]
    GameObject gameLostPanel;
    [SerializeField]
    Button restartGameButton;

    [SerializeField] Text guideText;

    #region CHAR butt
    [Header("Char  PORTRAIT")]
    public Button charButton;
    [SerializeField]
    Image charPortrait;
    PlayerControls charToShow;
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
    Sprite speedIcon;
    [SerializeField]
    Sprite lifeIcon;
    [SerializeField]
    Sprite manaIcon;
    [SerializeField]
    Sprite defenseIcon;
    [SerializeField]
    Sprite offenseIcon;
    #endregion

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();

        AddButtListeners();
        ShowLevelUpPopup(new PlayerControls(), false);
        ShowWarningPopup(false);
        DisplayGameLostPopup(false);
        ShowCharPopup(new PlayerControls(), false);

        startScreen.SetActive(true);
        darkVictoryScreen.SetActive(false);
        lightVictoryScreen.SetActive(false);

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
        if(playerToLevel.stats.UpdateProgressToGameVictory(ExpAction.LevelUpDark))
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

    public void DisplayGameLostPopup(bool show = true)
    {
        if (!show)
        {
            gameLostPanel.SetActive(false);
            return;
        }
        gameLostPanel.SetActive(true);
    }

    public void UpdateGuideText(string newText)
    {
        guideText.text = newText;
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
         expText.text =  playerControls.stats.currExp.ToString() + "/"+ playerControls.stats.expRequired.ToString() + "xp";

         offenseText.text = "Offense: " + playerControls.stats.offense.ToString();
         armorText.text = "Defense: " + playerControls.stats.defense.ToString();

         lifeText.text = playerControls.stats.currLife.ToString() + "/" + playerControls.stats.maxLife.ToString();
         manaText.text = "Mana: " + playerControls.stats.currMana.ToString() + "/" + playerControls.stats.maxMana.ToString();

        speedText.text = "Speed: " + playerControls.stats.speed;

        switch (playerControls.type)
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
        if (player.type != CharType.Player)
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
}
