using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    TurnManager turnManager;
    public PopupManager popupManager;
    [SerializeField]
    CameraController cameraController;

    [Header("UI")]
    [SerializeField] Button endTurnButton;

    [SerializeField] Button skillButton;
    Text skillButtonText;

    [SerializeField] GameObject targetHighlight;
    List<HighlightTileObject> skillHighlights = new List<HighlightTileObject>();

    #region CHAR MANAGEMENT
    public List<PlayerControls> allCharacters = new List<PlayerControls>(); // all characters currently in game, including NPCS
    public PlayerControls selectedCharacter;
    private bool isAnyCharSelected = false;
    public List<Vector2Int> allowedWalkCoordsNPC = new List<Vector2Int>();
    #endregion

    #region SKILLS
    bool skillSelected = false;
    [HideInInspector]
    public Skill selectedSkill;
    private int targetHighlightCounter = 0;
    Image skillButtonImage;

    Color skillButtonDefaultColor = Color.white;
    Color skillButtonSelectedColor = Color.red;
    #endregion

    #region ENVIRONMENT
    [Header("ENVIRONMENT")]
    public LevelBounds levelTopBorder;
    public LevelBounds levelBottomBorder;
    public LevelBounds levelRightBorder;
    public LevelBounds levelLeftBorder;
    [HideInInspector]
    public List<Obstacle> allObstacles = new List<Obstacle>();
    #endregion

    private void Awake()
    {
        if (GameData.current == null)
            GameData.current = new GameData();
        GetComponents();
    }

    private void GetComponents()
    {
        turnManager = gameObject.GetComponent<TurnManager>();
        popupManager = gameObject.GetComponent<PopupManager>();
    }

    private void Start()
    {
        // INITIALIZE CHARACTERS
        foreach (PlayerControls character in FindObjectsOfType<PlayerControls>())
        {
            allCharacters.Add(character);
            NPC npcController = character.GetComponent<NPC>();
            npcController.gameManager = this;
            npcController.npcControls = character;
        }

        foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
        {
            allObstacles.Add(obstacle);
        }

        // INITIALIZE BUTTONS
        endTurnButton.onClick.AddListener(EndTurn);
        InitializeSkillButton();
    }

    private void ShowEndTurnButton(bool show = true)
    {
        endTurnButton.gameObject.SetActive(show);
    }

    public void ProcessShowCharNameRequest(PlayerControls character)
    {
        if (!skillSelected && !isAnyCharSelected)
        {
            switch (character.type)
            {
                case CharType.Enemy:
                    popupManager.UpdateGuideText(character.name + " (Enemy)");
                    break;
                case CharType.Player:
                    popupManager.UpdateGuideText(character.name);
                    break;
                case CharType.Neutral:
                    popupManager.UpdateGuideText(character.name + " (Neutral)");
                    break;
            }
        }
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
            if (selectedCharacter.hasUsedSkillThisTurn)
            {
                popupManager.UpdateGuideText(selectedCharacter.name + " can only act once per turn!");
                return;
            }
            selectedSkill = selectedCharacter.stats.skills[0];
            skillButtonImage.color = skillButtonSelectedColor;
            DisplayActionRange();
        }
        else
        {
            skillButtonImage.color = skillButtonDefaultColor;
            DisplayActionRange(ActionType.Walk);
        }
    }

    public void DisplayActionRange(ActionType actionType = ActionType.UseCombatSkill, CharType charType = CharType.Player)
    {
        // show the char sprite above the tile sprites
        selectedCharacter.SetToDefaultSortOrder(defaultOrder: false);

        int actionRange;
        if (actionType == ActionType.UseCombatSkill)
        {
            popupManager.UpdateGuideText("Use " + selectedSkill.name);
            actionRange = selectedSkill.skillRange;
        }
        else
        {
            int speedLeft = selectedCharacter.playerSpeed - selectedCharacter.tilesWalked;
            if (charType == CharType.Player)
            {
                if (speedLeft > 0)
                    popupManager.UpdateGuideText("Select Destination");
                else
                    popupManager.UpdateGuideText("No moves left");
            }
            actionRange = speedLeft;
        }

        bool reuseOldTargetHighlights = false;
        targetHighlightCounter = 0;
        if (skillHighlights.Count > 0)
        {
            reuseOldTargetHighlights = true;
        }

        if (GameData.current.turnType != TurnType.Player)
        {
            allowedWalkCoordsNPC.Clear();

            // spawn a tile below the npc
            if (actionType == ActionType.Walk)
            {
                Vector3 highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
            }
        }

        for (int i = 1; i < actionRange + 1; i++)
        {
            // spawn up
            ProcessSpawnHighlightTileRequest(
                xOffset: 0,
                yOffset: i,
                reuseOldTargetHighlights,
                actionType);



            // spawn up-right && up-left
            for (int z = 1; z < actionRange - i + 1; z++)
            {
                ProcessSpawnHighlightTileRequest(
                    xOffset: -z,
                    yOffset: +i,
                    reuseOldTargetHighlights,
                    actionType);

                ProcessSpawnHighlightTileRequest(
                    xOffset: z,
                    yOffset: i,
                    reuseOldTargetHighlights,
                    actionType);
            }

            //spawn down
            ProcessSpawnHighlightTileRequest(
                    xOffset: 0,
                    yOffset: -i,
                    reuseOldTargetHighlights,
                    actionType);

            // spawn down-right && down-left
            for (int z = 1; z < actionRange - i + 1; z++)
            {
                ProcessSpawnHighlightTileRequest(
                    xOffset: -z,
                    yOffset: -i,
                    reuseOldTargetHighlights,
                    actionType);

                ProcessSpawnHighlightTileRequest(
                    xOffset: +z,
                    yOffset: -i,
                    reuseOldTargetHighlights,
                    actionType);
            }

            // spawn right
            ProcessSpawnHighlightTileRequest(
                xOffset: +i,
                yOffset: 0,
                reuseOldTargetHighlights,
                actionType);

            // spawn left
            ProcessSpawnHighlightTileRequest(
                xOffset: -i,
                yOffset: 0,
                reuseOldTargetHighlights,
                actionType);
        }
    }

    private void ProcessSpawnHighlightTileRequest(int xOffset, int yOffset, bool reuseOldTargetHighlights, ActionType actionType)
    {
        
        Vector3 highlightLocation = new Vector3(selectedCharacter.transform.position.x + xOffset,
                selectedCharacter.transform.position.y + yOffset, selectedCharacter.transform.position.z);

        // DONT SPAWN IF OUTSIDE LEVEL BOUNDS
        if (highlightLocation.x < levelLeftBorder.xCoord)
            return;
        if (highlightLocation.x > levelRightBorder.xCoord)
            return;
        if (highlightLocation.y < levelBottomBorder.yCoord)
            return;
        if (highlightLocation.y > levelTopBorder.yCoord)
            return;

        // DONT SPAWN IF ON TOP OF OBStacle        
        foreach (Obstacle obstacle in allObstacles)
        {
            if (obstacle.pos.x == (int)highlightLocation.x && obstacle.pos.y == (int)highlightLocation.y)
                return;
        }


        // DONT SPAWN ON TOP OF CHAR IF THAT'S MOVE ACTION
        foreach (PlayerControls character in allCharacters)
        {
            if (actionType != ActionType.Walk)
                break;
            if (!character.isDead && character.xCoord == (int)highlightLocation.x && character.yCoord == (int)highlightLocation.y)
                return;
        }

        allowedWalkCoordsNPC.Add(new Vector2Int((int)highlightLocation.x, (int)highlightLocation.y));
        SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
    }

    private void HideActionRange()
    {
        if (isAnyCharSelected)
            UpdateRemainingMovesText(selectedCharacter.playerSpeed - selectedCharacter.tilesWalked);
        if (skillSelected)
            skillButtonImage.color = skillButtonDefaultColor;

        foreach (HighlightTileObject skillHighlight in skillHighlights)
        {
            skillHighlight.ShowTile(ActionType.Walk, false);
        }
        foreach (PlayerControls character in allCharacters)
        {
            character.SetToDefaultSortOrder(true);
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
                break;
        }

    }

    private void Update()
    {
        if (GameData.current.turnType != TurnType.Player)
            return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCharInfoPanel();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EndTurn();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && popupManager.isWarningPopupActive)
        {
            popupManager.ShowWarningPopup(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && isAnyCharSelected)
        {
            SelectSkill();
        }
    }

    public void UpdateRemainingMovesText(int remainingMoves)
    {
        if (GameData.current.turnType != TurnType.Player)
            return;
        popupManager.UpdateGuideText("Remaining moves: " + (remainingMoves).ToString());
    }



    /// <summary>
    /// Returns true if character selected successfully
    /// </summary>
    /// <param name="characterToSelect"></param>
    /// <returns></returns>
    public bool SelectCharacter(PlayerControls characterToSelect)
    {
        bool characterSelected = false;

        if (characterToSelect.type == CharType.Player && GameData.current.turnType != TurnType.Player)
            return characterSelected;

        foreach (PlayerControls character in allCharacters)
        {
            if (character == characterToSelect)
            {
                isAnyCharSelected = true;
                character.SelectCharacter(true);
                UpdateRemainingMovesText(character.playerSpeed - character.tilesWalked);
                characterSelected = true;
                selectedCharacter = characterToSelect;
                HideActionRange();
                if (character.type != CharType.Player)
                {
                    return characterSelected;
                }
                DisplayActionRange(ActionType.Walk, character.type);
                ShowSkillButton();
                return characterSelected;
            }
            else
            {
                character.SelectCharacter(false);
            }
        }
        return characterSelected;
    }

    private void ShowSkillButton(bool show = true)
    {
        skillButton.gameObject.SetActive(show);
        if (show)
            skillButtonText.text = selectedCharacter.stats.skills[0].name;
    }

    /// <summary>
    /// Returns true if there's any character in the player's party that can move or use ability
    /// </summary>
    /// <returns></returns>
    private bool CanPlayerPartyAct()
    {
        foreach (PlayerControls character in allCharacters)
        {
            if (character.type == CharType.Player &&
                character.CanCharacterAct())
                return true;
        }
        return false;
    }

    private void MoveCameraToPlayer()
    {
        cameraController.SetPosition(new Vector3(
            selectedCharacter.transform.position.x,
            selectedCharacter.transform.position.y+1,
            cameraController.transform.position.z));
    }

    public bool CheckForAlivePlayers()
    {
        foreach (PlayerControls character in allCharacters)
        {
            if (character.type == CharType.Player && !character.isDead)
            {
                return true;
            }
        }
        return false;
    }

    private void EndGame()
    {
        popupManager.DisplayGameLostPopup();
    }

    public void EndTurn()
    {
        // CHECK IF ANY PLAYERS ARE ALIVE
        if (!CheckForAlivePlayers())
        {
            Debug.Log("game should end");
            EndGame();
            return;
        }


        // SHOW CONFIRMATION POPUP
        if (GameData.current.turnType == TurnType.Player)
        {
            //if (!popupManager.isWarningPopupActive && CanPlayerPartyAct())
            //{
            //    popupManager.ShowWarningPopup(true);
            //    return;
            //}
            //else
            //{
            //    popupManager.ShowWarningPopup(false);
            //}
        }

        // START NEW TURN
        turnManager.StartNewTurn();
        ShowManaRegenTexts();

        // HIDE IRRELEVANT UI
        HideActionRange();
        ShowSkillButton(false);
        if (GameData.current.turnType != TurnType.Player)
            ShowEndTurnButton(false);
        else
            ShowEndTurnButton(true);
       
        // RESET CHARACTER STATS
        foreach (PlayerControls character in allCharacters)
        {
            character.UpdateNewTurnStats();
        }

        // SELECT FIRST RANDOM PLAYER CHARACTER
        if (GameData.current.turnType == TurnType.Player)
        {
            foreach (PlayerControls character in allCharacters)
            {
                if (character.type == CharType.Player)
                {
                    SelectCharacter(character);
                    MoveCameraToPlayer();
                    break;
                }
            }

        // SHOW CHAR UI IF PLAYER TURN
            ShowSkillButton(true);
            DisplayActionRange(ActionType.Walk);
            UpdateRemainingMovesText(selectedCharacter.playerSpeed);
        }

        // MAKE NEUTRAL/ENEMY CHARS ACT
        if (GameData.current.turnType == TurnType.Player)
            return;
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if ((allCharacters[i].type == CharType.Neutral &&
                GameData.current.turnType == TurnType.Neutral)
                ||
                (allCharacters[i].type == CharType.Enemy &&
                GameData.current.turnType == TurnType.Enemy))
            {
                allCharacters[i].npcController.id = i;
                SelectCharacter(allCharacters[i]);
                allCharacters[i].npcController.Act();
                return;
            }
        }
        EndTurn();
    }


    /// <summary>
    /// Called by NPC when they finish their actions for the turn
    /// </summary>
    /// <param name="charType"></param>
    /// <param name="currCharID">assigned by game manager script in end turn function </param>
    public void ProcessEndCharMove(CharType charType, int currCharID)
    {
        for (int i = currCharID + 1; i < allCharacters.Count; i++)
        {
            if ((allCharacters[i].type == CharType.Neutral &&
                GameData.current.turnType == TurnType.Neutral)
                ||
                (allCharacters[i].type == CharType.Enemy &&
                GameData.current.turnType == TurnType.Enemy))
            {
                allCharacters[i].npcController.id = i;
                SelectCharacter(allCharacters[i]);
                allCharacters[i].npcController.Act();
                return;
            }
        }
        EndTurn();
    }


    /// <summary>
    /// Check whether panel was opened by mouse so that you don't accidentally close it with mouse click
    /// </summary>
    /// <param name="toggledByMouse"></param>
    public void ToggleCharInfoPanel(bool toggledByMouse = false)
    {
        popupManager.ShowCharPopup(selectedCharacter);
    }

    /// <summary>
    /// Attempt to use selected skill on X;Y coordinates
    /// </summary>
    /// <param name="xCoordinate"></param>
    /// <param name="yCoordinate"></param>
    public void ProcessInteractionRequest(int xCoordinate, int yCoordinate, ActionType actionType)
    {
        if (GameData.current.turnType != TurnType.Player)
            return;

        // WALK INTERACTION
        if (actionType == ActionType.Walk)
        {

            // 1 - check if tile not occupied
            foreach (PlayerControls character in allCharacters)
            {
                if (xCoordinate == character.xCoord && yCoordinate == character.yCoord)
                {
                    Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            foreach (Obstacle obstacle in allObstacles)
            {
                if (xCoordinate == obstacle.pos.x && yCoordinate == obstacle.pos.y)
                {
                    Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            HideActionRange();
            selectedCharacter.TeleportCharacter(xCoordinate, yCoordinate);
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
            popupManager.UpdateGuideText("Invalid target");
            return;
        }
        // 2 - check if enough mana
        if (selectedCharacter.stats.currMana < selectedSkill.manaCost)
        {
            popupManager.UpdateGuideText("Not enough mana!");
            return;
        }

        // 2.5 - hide current skill highlights
        SelectSkill();
        HideActionRange();


        // 3 - apply skill effect
        float damageDealt = target.TakeDamage(-selectedSkill.skillDamage, selectedCharacter);
        selectedCharacter.hasUsedSkillThisTurn = true;

        // 4 - remove spent mana
        selectedCharacter.SpendMana(selectedSkill.manaCost);
            
    }


    public void RestartGame()
    {
        GameData.current = new GameData();
        SceneManager.LoadScene(0);
    }


    public bool IsTileAllowedForNPC(Vector2Int coord)
    {
        foreach (Vector2Int currCoord in allowedWalkCoordsNPC)
        {
            if (currCoord == coord)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsTileOccupiedByObstacle(Vector2Int coord)
    {
        foreach (Obstacle obstacle in allObstacles)
        {
            if (obstacle.pos.x == coord.x && obstacle.pos.y == coord.y)
            {
                Debug.Log(coord + " occuppied by obstacle");
                return true;
            }
        }
        Debug.Log("game manage " + coord + " not occupied by obstacle");
        return false;
    }

    public void ShowManaRegenTexts()
    {
        foreach (PlayerControls character in allCharacters)
        {
            switch (GameData.current.turnType)
            {
                case TurnType.Neutral:
                    if (character.type == CharType.Neutral)
                        character.RegenMana();
                    break;
                case TurnType.Enemy:
                    if (character.type == CharType.Enemy)
                        character.RegenMana();
                    break;
                case TurnType.Player:
                    if (character.type == CharType.Player)
                        character.RegenMana();
                    break;
            }
        }
    }

    public void Victory()
    {
        StartCoroutine(DisplayVictoryAfterDelay(0.5f));
    }

    private IEnumerator DisplayVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameData.current.currMoonPoints < 0)
        {
            popupManager.ShowDarkVictory();
            Debug.Log("VICTORY dark");
        }
        else if (GameData.current.currMoonPoints > 0)
        {
            popupManager.ShowLightVictory();
            Debug.Log("VICTORY light");
        }
    }
}
