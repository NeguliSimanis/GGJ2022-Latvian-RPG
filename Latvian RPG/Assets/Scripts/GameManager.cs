using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType
{
    UseCombatSkill,
    Walk
}

public class HighlightTileObject
{
    public int ID;
    public GameObject highlightObject;
    private Color transparent = new Color(1, 1, 1, 0);
    private Color red = new Color(1, 0.3f, 0.3f, 0.4f);
    private SpriteRenderer spriteRenderer;
    private TileHighlight tileHighlight;

    public HighlightTileObject(int newID, GameObject newObject)
    {
        ID = newID;
        highlightObject = newObject;
        spriteRenderer = highlightObject.GetComponent<SpriteRenderer>();
        tileHighlight = highlightObject.GetComponent<TileHighlight>();
    }

    public void ShowTile(ActionType actionType, bool show)
    {
        tileHighlight.EnableTile(actionType, show);
    }
}

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    GameObject charInfoPanel;
    [SerializeField]
    Button endTurnButton;
    [SerializeField]
    Text currentTurnText;
    [SerializeField]
    Text guideText;
    [SerializeField]
    Button skillButton;
    Text skillButtonText;
    List<PlayerControls> allCharacters = new List<PlayerControls>(); // all characters currently in game, including NPCS
    [SerializeField]
    GameObject targetHighlight;
    List<HighlightTileObject> skillHighlights = new List<HighlightTileObject>();

    public PlayerControls selectedCharacter;


    #region SKILLS
    bool skillSelected = false;
    Skill selectedSkill;
    private int targetHighlightCounter = 0;
    Image skillButtonImage;

    Color skillButtonDefaultColor = Color.white;
    Color skillButtonSelectedColor = Color.red;
    #endregion

    private void Awake()
    {
        if (GameData.current == null)
            GameData.current = new GameData();
    }

    private void Start()
    {
        foreach (PlayerControls character in FindObjectsOfType<PlayerControls>())
        {
            allCharacters.Add(character);
        }
        endTurnButton.onClick.AddListener(EndTurn);
        InitializeSkillButton();
        UpdateTurnQueueHUD();
    }

    private void InitializeSkillButton()
    {
        skillButton.onClick.AddListener(SelectSkill);
        skillButtonText = skillButton.transform.GetChild(0).GetComponent<Text>();
        skillButtonImage = skillButton.gameObject.GetComponent<Image>();
        skillButton.gameObject.SetActive(false);
    }

    private void SelectSkill()
    {
        skillSelected = !skillSelected;
        HideActionRange();
        if (skillSelected)
        {
            selectedSkill = selectedCharacter.stats.skills[0];
            skillButtonImage.color = skillButtonSelectedColor;
            DisplayActionRange();
        }
        else
            DisplayActionRange(ActionType.Walk);
    }

    private void DisplayActionRange(ActionType actionType = ActionType.UseCombatSkill)
    {
        int actionRange;
        if (actionType == ActionType.UseCombatSkill)
        {
            guideText.text = "Select Target";
            actionRange = selectedSkill.skillRange;
        }
        else
        {
            int speedLeft = selectedCharacter.playerSpeed - selectedCharacter.tilesWalked;
            if (speedLeft > 0)
                guideText.text = "Select Destination";
            else
                guideText.text = "No moves left";
            actionRange = speedLeft;
        }

        bool reuseOldTargetHighlights = false;
        targetHighlightCounter = 0;
        if (skillHighlights.Count > 0)
        {
            reuseOldTargetHighlights = true;
        }


        for (int i = 1; i < actionRange + 1; i++)
        {
            // spawn up
            Vector3 highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);



            // spawn up-right && up-left
            for (int z = 1; z < actionRange - i + 1; z++)
            {
                highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);

                highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
            }

            //spawn down
            highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);



            // spawn down-right && down-left
            for (int z = 1; z < actionRange - i + 1; z++)
            {
                highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);

                highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
            }

            // spawn right
            highlightLocation = new Vector3(selectedCharacter.transform.position.x + i,
                selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);

            // spawn left
            highlightLocation = new Vector3(selectedCharacter.transform.position.x - i,
                selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
        }
    }

    private void HideActionRange()
    {
        UpdateRemainingMovesText(selectedCharacter.playerSpeed - selectedCharacter.tilesWalked);
        skillButtonImage.color = skillButtonDefaultColor;

        foreach (HighlightTileObject skillHighlight in skillHighlights)
        {
            skillHighlight.ShowTile(ActionType.Walk, false);
        }
    }

    private void SpawnHighlightTile(Vector3 highLightLocation, bool oldHighlightsExist, ActionType actionType)
    {
        bool createNewHighLights = true;
        targetHighlightCounter++;
        if (oldHighlightsExist)
        {
            if (targetHighlightCounter <= skillHighlights.Count)
                createNewHighLights = false;
        }

        switch (createNewHighLights)
        {
            case true:
                GameObject newHighlight = Instantiate(targetHighlight, highLightLocation, Quaternion.identity);
                newHighlight.name = "highlight" + targetHighlightCounter.ToString();
                HighlightTileObject highlightTileObject = new HighlightTileObject(targetHighlightCounter, newHighlight);
                skillHighlights.Add(highlightTileObject);
                highlightTileObject.ShowTile(actionType, true);
                break;


            case false:
                skillHighlights[targetHighlightCounter - 1].ShowTile(actionType, true);
                skillHighlights[targetHighlightCounter - 1].highlightObject.transform.position = highLightLocation;
                //foreach (HighlightTileObject skillHighlight in skillHighlights)
                //{
                //    skillHighlight.ShowTile();
                //}
                break;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCharInfoPanel();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EndTurn();
        }
    }

    public void UpdateRemainingMovesText(int remainingMoves)
    {
        guideText.text = "Remaining moves: " + (remainingMoves).ToString();
    }

    /// <summary>
    /// Returns true if character selected successfully
    /// </summary>
    /// <param name="characterToSelect"></param>
    /// <returns></returns>
    public bool SelectCharacter(PlayerControls characterToSelect)
    {
        bool characterSelected = false;
        foreach (PlayerControls character in allCharacters)
        {
            if (character == characterToSelect)
            {
                character.SelectCharacter(true);
                UpdateRemainingMovesText(character.playerSpeed - character.tilesWalked);
                characterSelected = true;
                selectedCharacter = characterToSelect;
                ShowSkillButton();
                HideActionRange();
                DisplayActionRange(ActionType.Walk);
            }
            else
                character.SelectCharacter(false);
        }
        return characterSelected;
    }

    private void ShowSkillButton()
    {
        skillButton.gameObject.SetActive(true);
        skillButtonText.text = selectedCharacter.stats.skills[0].name;
    }

    private void EndTurn()
    {
        GameData.current.currentTurn++;
        HideActionRange();
        foreach (PlayerControls character in allCharacters)
        {
            character.tilesWalked = 0;
        }
        currentTurnText.text = "Turn " + GameData.current.currentTurn.ToString();
        DisplayActionRange(ActionType.Walk);
        UpdateRemainingMovesText(selectedCharacter.playerSpeed);
    }


    /// <summary>
    /// Check whether panel was opened by mouse so that you don't accidentally close it with mouse click
    /// </summary>
    /// <param name="toggledByMouse"></param>
    public void ToggleCharInfoPanel(bool toggledByMouse = false)
    {
        if (!toggledByMouse)
            charInfoPanel.SetActive(!charInfoPanel.activeInHierarchy);
        else
            charInfoPanel.SetActive(true);
    }

    /// <summary>
    /// Attempt to use selected skill on X;Y coordinates
    /// </summary>
    /// <param name="xCoordinate"></param>
    /// <param name="yCoordinate"></param>
    public void ProcessInteractionRequest(int xCoordinate, int yCoordinate, ActionType actionType)
    {

        // WALK INTERACTION
        if (actionType == ActionType.Walk)
        {

            // 1 - check if tile not occupied
            foreach (PlayerControls character in allCharacters)
            {
                if (xCoordinate == character.xCoord && yCoordinate == character.yCoord)
                {
                    Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    guideText.text = "Cannot move there!";
                    return;
                }
            }
            selectedCharacter.TeleportCharacter(xCoordinate, yCoordinate);
            HideActionRange();
            UpdateRemainingMovesText(selectedCharacter.playerSpeed - selectedCharacter.tilesWalked);
            DisplayActionRange(ActionType.Walk);
            return;
        }


        // COMBAT INTERACTION
        bool targetViable = false;
        PlayerControls target = allCharacters[0];

        // 1 - check if viable target on coordinates
        foreach (PlayerControls character in allCharacters)
        {
            if (xCoordinate == character.xCoord && yCoordinate == character.yCoord)
            {
                Debug.Log("TARGET ACQUIRED AT " + xCoordinate + ":" + yCoordinate);
                target = character;
                targetViable = true;

            }
        }
        if (!targetViable)
        {
            Debug.Log("NOOO TARGET AT CH" + xCoordinate + ":" + yCoordinate);
            guideText.text = "Invalid target";
            return;
        }
        // 2 - check if enough mana


        // 2.5 - hide current skill highlights
        SelectSkill();
        HideActionRange();


        // 3 - apply skill effect
        float damageDealt = target.TakeDamage(-selectedSkill.skillDamage, selectedCharacter);

    }

    public void UpdateGuideText(string newText)
    {
        guideText.text = newText;
    }

    #region TURN QUEUE HUD


    private int maxElementsInTurnQueue = 15;
    [Header("TURN QUEUE")]
    [SerializeField]
    GameObject queuePortraitPrefab;
    [SerializeField]
    GameObject queueTurnInfo;
    [SerializeField]
    Transform queueParentTransform;
    private void UpdateTurnQueueHUD()
    {
        int currElementCount = 0;
        int turnCounter = 0;
        while (currElementCount < maxElementsInTurnQueue)
        {
            // ADD CHAR INFO
            currElementCount += AddCharInfoToQueue(GameData.current.currentTurn + turnCounter);

            // ADD TURN INFO
            AddTurnInfoToQueue(GameData.current.currentTurn + turnCounter + 1);
            turnCounter++;
            currElementCount++;
        }
    }

    private int AddCharInfoToQueue(int turn)
    {
        int charsAdded = 0;
        Debug.Log("what");
        foreach(PlayerControls character in allCharacters)
        {
            Debug.Log("YO!");
            GameObject newCharPortrait = Instantiate(queuePortraitPrefab, queueParentTransform);
            Text newCharText = newCharPortrait.transform.GetChild(0).gameObject.GetComponent<Text>();
            newCharText.text = character.name;
            charsAdded++;
        }
        return charsAdded;
    }

    private void AddTurnInfoToQueue(int turn)
    {
        GameObject newTurnInfo = Instantiate(queueTurnInfo, queueParentTransform);
        Text newTurnText = newTurnInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
        newTurnText.text = "Turn " + (turn);
    }

    #endregion
}
