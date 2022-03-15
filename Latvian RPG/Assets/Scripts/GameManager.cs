using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System.Diagnostics;


public enum ActionType
{
    UseCombatSkill,
    Walk,
    UseUtilitySkill,
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

    public void ShowTile(ActionType actionType, bool show, bool allowInteraction = true)
    {
        tileHighlight.EnableTile(actionType, show, allowInteraction);
    }
}

public class GameManager : MonoBehaviour
{

    TurnManager turnManager;
    public PopupManager popupManager;
    [SerializeField]
    CameraController cameraController;
    public AudioManager audioManager;


    [Header("UI")]
    [SerializeField]
    GameObject turnTimerObject;
    [SerializeField]
    Image turnTimerFill1;
    [SerializeField]
    Image turnTimerFill2;

    [SerializeField] Button endTurnButton;

    [SerializeField] Button skillButton;
    [SerializeField] SkillButton skillButtonControls;
    Text skillButtonText;
    Image skillButtonImage;

    [SerializeField] Button skillButton2;
    [SerializeField] SkillButton skillButtonControls2;
    Text skillButtonText2;
    Image skillButtonImage2;

    int lastSelectedSkillButton = 0; // 0 - none , 1 - first , 2 - second

    [SerializeField] GameObject targetHighlight;
    List<HighlightTileObject> skillHighlights = new List<HighlightTileObject>();

    #region CHAR MANAGEMENT
    [SerializeField]
    private GameObject[] charRoster;
    public List<PlayerControls> allCharacters = new List<PlayerControls>(); // all characters currently in game, including NPCS
    public PlayerControls selectedChar;
    private PlayerControls lastSelectedPlayerChar;
    public PlayerControls highlightedChar;
    private bool isAnyCharSelected = false;
    public List<Vector2Int> allowedWalkCoordsNPC = new List<Vector2Int>();
    #endregion

    #region SKILLS
    bool skillSelected = false;
    [HideInInspector]
    public Skill selectedSkill;
    private int targetHighlightCounter = 0;


    Color skillButtonDefaultColor = Color.white;
    Color skillButtonSelectedColor = Color.red;
    #endregion

    #region ENVIRONMENT
    [Header("ENVIRONMENT")]
    [SerializeField]
    private GameObject [] dungeonFloors;
    [SerializeField]
    private GameObject currFloor;
    [HideInInspector]
    public LevelBounds levelTopBorder;
    [HideInInspector]
    public LevelBounds levelBottomBorder;
    [HideInInspector]
    public LevelBounds levelRightBorder;
    [HideInInspector]
    public LevelBounds levelLeftBorder;
    [HideInInspector]
    public List<Obstacle> allObstacles = new List<Obstacle>();
    public List<InteractableObject> levelObjects = new List<InteractableObject>();
    #endregion

    private void Awake()
    {
        if (GameData.current == null)
            GameData.current = new GameData();
        GetComponents();
    }

    public void HighlightChar(PlayerControls charToHighlight, bool highlight = true)
    {
        // UNHIGHLIGHT THE PREVIOUSLY HIGHLIGHTED CHARACTER
        highlightedChar.charMarker.AnimateMarker(false);
        //if (!GameData.current.gameStarted)
        //    return;
        if (!highlight)
        {
            highlightedChar = selectedChar;
        }
        else
        {
            highlightedChar = charToHighlight;
        }
        charToHighlight.charMarker.AnimateMarker(highlight);
        popupManager.UpdateCharButton(highlightedChar);
    }

    private void GetComponents()
    {
        turnManager = gameObject.GetComponent<TurnManager>();
        popupManager = gameObject.GetComponent<PopupManager>();
    }

    public void StartGame()
    {
        SpawnNewFloor();
        SpawnRandomStartingChar();
        //SpawnStartingChar(Character.Goat);
        //foreach (PlayerControls playerControls in allCharacters)
        //{
        //    if (playerControls.type == CharType.Player)
        //    {
        //        Debug.Log("STARTING FOUND");
        //        highlightedChar = playerControls;
        //        HighlightChar(playerControls);
        //        SelectChar(playerControls);
        //        return;
        //    }
        //}
        GameData.current.gameStarted = true;
        GameData.current.playerTurnStartTime = Time.time;
        GameData.current.playerTurnEndTime = GameData.current.playerTurnStartTime + GameData.current.playerTurnTimer + 4f;
    }

    public void SpawnRandomStartingChar()
    {
        int rosterLength = charRoster.Length;
        Character randomCharacter;
        int randomRoll = Random.Range(0, rosterLength);
        randomCharacter = charRoster[randomRoll].GetComponent<PlayerControls>().character;
        SpawnStartingChar(randomCharacter);
    }

    public void SpawnStartingChar(Character newCharacter)
    {
        GameObject newPlayer = charRoster[0];
        foreach (GameObject currChar in charRoster)
        {
            if (currChar.GetComponent<PlayerControls>().character == newCharacter)
            {
                newPlayer = currChar;
            }
        }
        GameObject newPlayerInstance = Instantiate(newPlayer);
        PlayerControls newControls = newPlayerInstance.GetComponent<PlayerControls>();
        newControls.type = CharType.Player;
        cameraController.startTarget = newPlayerInstance.transform;
        allCharacters.Add(newControls);

        highlightedChar = newControls;
        HighlightChar(newControls, highlight:true);
        SelectChar(newControls);
        cameraController.IntializeCamera(newPlayerInstance.transform);
        newControls.charMarker.UpdateMarkerColor(CharType.Player);
    }

    private void ShowTurnTimerBar(bool show = true)
    {
        turnTimerObject.SetActive(show);
        if (show)
        {
            GameData.current.playerTurnStartTime = Time.time;
            GameData.current.playerTurnEndTime = GameData.current.playerTurnStartTime + GameData.current.playerTurnTimer;
        }
    }

    private void UpdateTurnTimeBar()
    {
        turnTimerFill1.fillAmount = Mathf.InverseLerp(GameData.current.playerTurnEndTime, GameData.current.playerTurnStartTime, Time.time);
        turnTimerFill2.fillAmount = Mathf.InverseLerp(GameData.current.playerTurnEndTime, GameData.current.playerTurnStartTime, Time.time);
        //(GameData.current.playerTurnTimer / (Time.time - GameData.current.playerTurnStartTime));
        //(stats.currLife * 1f) / stats.maxLife;
    }

    private void Start()
    {
        // INITIALIZE CHARACTERS
        foreach (PlayerControls character in FindObjectsOfType<PlayerControls>())
        {
            AddNewCharacter(character);
        }
        FindFloorObstacles();
        FindFloorObjects();
        // INITIALIZE BUTTONS
        endTurnButton.onClick.AddListener(EndTurn);
        InitializeSkillButton();
    }

    public void AddNewCharacter(PlayerControls character)
    {
        allCharacters.Add(character);
        UnityEngine.Debug.Log("adding " + character.name);
        NPC npcController = character.GetComponent<NPC>();
        npcController.gameManager = this;
        npcController.npcControls = character;
    }

    private void FindFloorObstacles()
    {
        allObstacles.Clear();
        foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
        {
            allObstacles.Add(obstacle);
        }
    }
    private void FindFloorObjects()
    {
        levelObjects.Clear();
        foreach (InteractableObject interObject in FindObjectsOfType<InteractableObject>())
        {
            levelObjects.Add(interObject);
        }
    }

    public ObjectType CheckInteractableObject(Vector2Int coord)
    {
        foreach (InteractableObject iObject in levelObjects)
        {
            if (coord.x == iObject.xCoord && coord.y == iObject.yCoord)
            {
                if (iObject.objType == ObjectType.HealingPotion)
                {
                    iObject.consumed = true;
                    iObject.gameObject.SetActive(false);
                    return ObjectType.HealingPotion;
                }
                else
                {
                    return ObjectType.LevelExit;
                }
            }
        }
        return ObjectType.Undefined;
    }

    /// <summary>
    /// REMOVE CHARS THAT STAY IN OLD LEVEL
    /// </summary>
    public void RemoveOldNPCs()
    {
        int listLength = allCharacters.Count;
        for (int i = 0; i < listLength; i++)
        {
            if (allCharacters[i].type != CharType.Player)
            {
                PlayerControls removeThis = allCharacters[i];
                allCharacters.Remove(removeThis);
                UnityEngine.Debug.Log("removing " + removeThis.name);
                Destroy(removeThis.gameObject);
                listLength--;
                i--;
            }
        }
    }

    public void MovePlayerToFloorStartingPoint()
    {
        Transform levelStartPoint = currFloor.GetComponent<DungeonFloor>().levelStartPoint;
        Vector2Int startingPoint = new Vector2Int(
            (int)levelStartPoint.position.x,
            (int)levelStartPoint.position.y);
        Vector2Int startingPoint2 = new Vector2Int(startingPoint.x-1, startingPoint.y + 1);
        Vector2Int startingPoint3 = new Vector2Int(startingPoint.x-1, startingPoint.y - 1);

        Debug.Log("start poi " + startingPoint);
        int playerID = 0;
        foreach (PlayerControls curPlayer in allCharacters)
        {
            if (curPlayer.type == CharType.Player)
            {
                Debug.Log("TELEPORTING " + curPlayer.name);
                curPlayer.TeleportPlayerCharacter(startingPoint.x, startingPoint.y, instantTeleport: true);
                if (playerID == 1)
                    curPlayer.TeleportPlayerCharacter(startingPoint2.x, startingPoint2.y, instantTeleport: true);
                if (playerID == 2)
                    curPlayer.TeleportPlayerCharacter(startingPoint3.x, startingPoint3.y, instantTeleport: true);
                playerID++;
            }
        }
        // move camera
        MoveCameraToPlayer();
        
        
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
        skillButton.onClick.AddListener((delegate { SelectSkill(1); }));
        skillButtonText = skillButton.transform.GetChild(0).GetComponent<Text>();
        skillButtonImage = skillButton.gameObject.GetComponent<Image>();
        skillButton.gameObject.SetActive(false);

        skillButton2.onClick.AddListener((delegate { SelectSkill(2); }));
        skillButtonText2 = skillButton2.transform.GetChild(0).GetComponent<Text>();
        skillButtonImage2 = skillButton2.gameObject.GetComponent<Image>();
        skillButton2.gameObject.SetActive(false);
    }

    private void SelectSkill(int skillID = 0)
    {

        HideActionRange();

        // DETERMINE IF A BUTTON WAS SELECTED BEFORE

        if (skillSelected && lastSelectedSkillButton == skillID)
        {
            skillSelected = false;
        }
        else
            skillSelected = true;
        lastSelectedSkillButton = skillID;


        // SKILL DESELECTED
        if (!skillSelected)
        {
            skillButtonImage.color = skillButtonDefaultColor;
            skillButtonImage2.color = skillButtonDefaultColor;
            DisplayActionRange(ActionType.Walk);
            UnityEngine.Debug.Log("display walk");
            return;
        }


        if (selectedChar.hasUsedSkillThisTurn)
        {
            popupManager.UpdateGuideText(selectedChar.name + " can use skill only once per turn!");
            return;
        }

        // FIRST BUTTON
        if (skillID == 1)
        {
            selectedSkill = selectedChar.stats.skills[0];
            skillButtonImage.color = skillButtonSelectedColor;
            skillButtonImage2.color = skillButtonDefaultColor;
            if (selectedSkill.type[0] == SkillType.Damage)
                DisplayActionRange();
            else
            {
                DisplayActionRange(ActionType.UseUtilitySkill);
            }
            
        }
        if (skillID != 2)
            return;

        // SECOND BUTTON
        selectedSkill = selectedChar.stats.skills[1];
        skillButtonImage2.color = skillButtonSelectedColor;
        skillButtonImage.color = skillButtonDefaultColor;
        if (selectedSkill.type[0] == SkillType.Damage)
            DisplayActionRange();
        else
        {
            DisplayActionRange(ActionType.UseUtilitySkill);
        }
        
    }

    public void DisplayActionRange(ActionType actionType = ActionType.UseCombatSkill, CharType charType = CharType.Player)
    {
        
        // show the char sprite above the tile sprites
        selectedChar.SetToDefaultSortOrder(defaultOrder: false);

        int actionRange;
        if (actionType == ActionType.UseCombatSkill || actionType == ActionType.UseUtilitySkill)
        {
            popupManager.UpdateGuideText("Use " + selectedSkill.name);
            UnityEngine.Debug.Log("Use " + selectedSkill.name);
            actionRange = selectedSkill.skillRange;
        }
        else
        {
            int speedLeft = selectedChar.playerSpeed - selectedChar.tilesWalked;
            if (charType == CharType.Player)
            {
                if (speedLeft > 0)
                    popupManager.UpdateGuideText("Select Destination");
                else
                    popupManager.UpdateGuideText("No moves left");
            }
            actionRange = speedLeft;
        }
        UnityEngine.Debug.Log("display " + actionType + ". Range =" + actionRange);

        bool reuseOldTargetHighlights = false;
        targetHighlightCounter = 0;
        if (skillHighlights.Count > 0)
        {
            reuseOldTargetHighlights = true;
        }
        
        if (GameData.current.turnType != CharType.Player)
        {
            allowedWalkCoordsNPC.Clear();
        }

        // spawn a tile below the character
        if ((actionType == ActionType.Walk && selectedChar.tilesWalked < selectedChar.playerSpeed)
            || (actionType == ActionType.UseUtilitySkill))
        {
            Vector3 highlightLocation = new Vector3(selectedChar.transform.position.x,
            selectedChar.transform.position.y, selectedChar.transform.position.z);
            if (actionType != ActionType.UseUtilitySkill)
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType, allowInteraction: false);
            else
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType, allowInteraction: true);
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

        Vector3 highlightLocation = new Vector3(selectedChar.transform.position.x + xOffset,
                selectedChar.transform.position.y + yOffset, selectedChar.transform.position.z);

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

    public void HideActionRange()
    {

        //if (isAnyCharSelected)
        //    UpdateRemainingMovesText(selectedChar.playerSpeed - selectedChar.tilesWalked);
        if (skillSelected)
        {
            skillButtonImage.color = skillButtonDefaultColor;
            skillButtonImage2.color = skillButtonDefaultColor;
        }

        foreach (HighlightTileObject skillHighlight in skillHighlights)
        {
            skillHighlight.ShowTile(ActionType.Walk, false);
        }
        foreach (PlayerControls character in allCharacters)
        {
            character.SetToDefaultSortOrder(true);
        }
    }

    private void SpawnHighlightTile(Vector3 highLightLocation, bool oldHighlightsExist, ActionType actionType, bool allowInteraction = true)
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
                highlightTileObject.ShowTile(actionType, true, allowInteraction);
                break;


            case false:
                skillHighlights[targetHighlightCounter - 1].ShowTile(actionType, true, allowInteraction);
                skillHighlights[targetHighlightCounter - 1].highlightObject.transform.position = highLightLocation;
                break;
        }

    }

    private void Update()
    {
        if (!GameData.current.gameStarted)
            return;
        if (GameData.current.turnType != CharType.Player)
            return;
        UpdateTurnTimeBar();
        if (Time.time > GameData.current.playerTurnEndTime)
        {
            EndTurn();
            return;
        }
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
        if (GameData.current.turnType != CharType.Player)
            return;
        popupManager.UpdateGuideText("Remaining moves: " + (remainingMoves).ToString());
    }



    /// <summary>
    /// Returns true if character selected successfully
    /// </summary>
    /// <param name="characterToSelect"></param>
    /// <returns></returns>
    public bool SelectChar(PlayerControls characterToSelect)
    {
        bool characterSelected = false;

        if (characterToSelect.type == CharType.Player && GameData.current.turnType != CharType.Player)
            return characterSelected;

        foreach (PlayerControls character in allCharacters)
        {
            if (character == characterToSelect)
            {
                isAnyCharSelected = true;
                character.SelectCharacter(true);
                UpdateRemainingMovesText(character.playerSpeed - character.tilesWalked);
                characterSelected = true;
                selectedChar = characterToSelect;
                HideActionRange();
                if (character.type != CharType.Player)
                {
                    return characterSelected;
                }
                lastSelectedPlayerChar = selectedChar;
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
        skillButton2.gameObject.SetActive(show);

        if (show)
        {
            Debug.Log(selectedChar);
            skillButtonText.text = selectedChar.stats.skills[0].name;
            skillButtonText2.text = selectedChar.stats.skills[1].name;
            skillButtonControls.skill = selectedChar.stats.skills[0];
            skillButtonControls2.skill = selectedChar.stats.skills[1];
        }
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

    private void MoveCameraToPlayer(bool instant = false)
    {
       
        Debug.Log(selectedChar.transform.position);
        cameraController.SetPosition(new Vector3(
            selectedChar.transform.position.x,
            selectedChar.transform.position.y + 1,
            cameraController.transform.position.z), instant);
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

    private void LoseGame()
    {
        audioManager.PlayDefeatSFX();
        popupManager.DisplayGameLostPopup();
    }

    public void EndTurn()
    {
        // CHECK IF ANY PLAYERS ARE ALIVE
        if (!CheckForAlivePlayers())
        {
            UnityEngine.Debug.Log("game should end");
            LoseGame();
            return;
        }


        // SHOW CONFIRMATION POPUP
        if (GameData.current.turnType == CharType.Player)
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
        if (GameData.current.turnType != CharType.Player)
        {
            popupManager.charButton.gameObject.SetActive(false);
            ShowEndTurnButton(false);
        }
        else
        {
            popupManager.charButton.gameObject.SetActive(true);
            ShowEndTurnButton(true);
        }

        // RESET CHARACTER STATS
        foreach (PlayerControls character in allCharacters)
        {
            if (character.type == GameData.current.turnType)
                character.UpdateNewTurnStats();
        }

        // SELECT FIRST RANDOM PLAYER CHARACTER
        if (GameData.current.turnType == CharType.Player)
        {
            GoToLastSelectedPlayerChar();

            // SHOW CHAR UI IF PLAYER TURN
            ShowTurnTimerBar(true);
            ShowSkillButton(true);
            DisplayActionRange(ActionType.Walk);
            UpdateRemainingMovesText(selectedChar.playerSpeed);
        }

        // MAKE NEUTRAL/ENEMY CHARS ACT
        if (GameData.current.turnType == CharType.Player)
            return;
        ShowTurnTimerBar(false);
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if ((allCharacters[i].type == CharType.Neutral &&
                GameData.current.turnType == CharType.Neutral)
                ||
                (allCharacters[i].type == CharType.Enemy &&
                GameData.current.turnType == CharType.Enemy))
            {
                allCharacters[i].npcController.id = i;
                SelectChar(allCharacters[i]);
                allCharacters[i].npcController.Act();
                return;
            }
        }
        EndTurn();
    }

    private void GoToLastSelectedPlayerChar()
    {
        foreach (PlayerControls character in allCharacters)
        {
            if (character.type == CharType.Player
                && character == lastSelectedPlayerChar
                && !character.isDead)
            {
                HighlightChar(character);
                SelectChar(character);
                MoveCameraToPlayer();
                return;
            }

        }
       
        // LAST SELECTED CHAR IS DEAD
        foreach (PlayerControls character in allCharacters)
        {
            if (character.type == CharType.Player
                && !character.isDead)
            {
                HighlightChar(character);
                SelectChar(character);
                MoveCameraToPlayer();
                break;
            }
        }
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
                GameData.current.turnType == CharType.Neutral)
                ||
                (allCharacters[i].type == CharType.Enemy &&
                GameData.current.turnType == CharType.Enemy))
            {
                allCharacters[i].npcController.id = i;
                SelectChar(allCharacters[i]);
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
        popupManager.ShowCharPopup(selectedChar);
    }

    /// <summary>
    /// Attempt to use selected skill on X;Y coordinates
    /// </summary>
    /// <param name="xCoordinate"></param>
    /// <param name="yCoordinate"></param>
    public void ProcessInteractionRequest(int xCoordinate, int yCoordinate, ActionType actionType)
    {
        if (GameData.current.turnType != CharType.Player)
            return;
        if (!GameData.current.gameStarted)
            return;

        // WALK INTERACTION
        if (actionType == ActionType.Walk)
        {

            // 1 - check if tile not occupied
            foreach (PlayerControls character in allCharacters)
            {
                if (xCoordinate == character.xCoord && yCoordinate == character.yCoord && !character.isDead)
                {
                    UnityEngine.Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            foreach (Obstacle obstacle in allObstacles)
            {
                if (xCoordinate == obstacle.pos.x && yCoordinate == obstacle.pos.y)
                {
                    UnityEngine.Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            selectedChar.TeleportPlayerCharacter(xCoordinate, yCoordinate);
            UpdateRemainingMovesText(selectedChar.playerSpeed - selectedChar.tilesWalked);
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
                UnityEngine.Debug.Log("TARGET ACQUIRED AT " + xCoordinate + ":" + yCoordinate);
                target = character;
                targetViable = true;

            }
        }
        if (!targetViable)
        {
            UnityEngine.Debug.Log("NOOO TARGET AT CH" + xCoordinate + ":" + yCoordinate);
            popupManager.UpdateGuideText("Invalid target");
            return;
        }
        // 2 - check if enough mana
        if (selectedChar.stats.currMana < selectedSkill.manaCost)
        {
            popupManager.UpdateGuideText("Not enough mana!");
            return;
        }

        // 2.5 - hide current skill highlights
        SelectSkill();
        HideActionRange();


        // 3 - apply skill effect
        if (selectedSkill.type[0] == SkillType.Damage)
        {
            target.TakeDamage(-selectedSkill.skillDamage, selectedChar);
            selectedChar.hasUsedSkillThisTurn = true;
        }
        else if (selectedSkill.type[0] == SkillType.Recruit)
        {
            if (target.Convert(CharType.Player))
            {
                selectedChar.GainExp(ExpAction.Hire);
                selectedChar.hasUsedSkillThisTurn = true;
            }
            }
        else if (selectedSkill.type[0] == SkillType.Buff)
        {
            foreach (SkillEffect skillEffect in selectedSkill.skillEffects)
            {
                GameObject newEffectObject = Instantiate(skillEffect.gameObject);
                SkillEffect newSkill = newEffectObject.GetComponent<SkillEffect>();
                target.activeStatusEffects.Add(newSkill);
                Debug.Log("Added " + skillEffect.name + " skill effect to " + target.name);
                target.ActivateStatusEffects();
            }
            selectedChar.hasUsedSkillThisTurn = true;
        }
        

        // 4 - remove spent mana
        selectedChar.SpendMana(selectedSkill.manaCost);

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
                UnityEngine.Debug.Log(coord + " occuppied by obstacle");
                return true;
            }
        }
        UnityEngine.Debug.Log("game manage " + coord + " not occupied by obstacle");
        return false;
    }

    public void ShowManaRegenTexts()
    {
        foreach (PlayerControls character in allCharacters)
        {
            switch (GameData.current.turnType)
            {
                case CharType.Neutral:
                    if (character.type == CharType.Neutral)
                        character.RegenMana();
                    break;
                case CharType.Enemy:
                    if (character.type == CharType.Enemy)
                        character.RegenMana();
                    break;
                case CharType.Player:
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
            audioManager.PlayDarkVictorySFX();
            popupManager.ShowDarkVictory();
            UnityEngine.Debug.Log("VICTORY dark");
        }
        else if (GameData.current.currMoonPoints > 0)
        {
            audioManager.PlayLightVictorySFX();
            popupManager.ShowLightVictory();
            UnityEngine.Debug.Log("VICTORY light");
        }
    }

    public void SpawnNewFloor()
    {
        Debug.Log("dungeon length " + dungeonFloors.Length);
        if (GameData.current.dungeonFloor == -1)
        {
            Debug.Log("yay");
            return;
        }
        DestroyImmediate(currFloor);
        int newFloorID = GameData.current.dungeonFloor;
        if (newFloorID >= dungeonFloors.Length)
        {
            newFloorID = dungeonFloors.Length - 1;
        }
        GameObject newFloor = Instantiate(dungeonFloors[newFloorID]);
        currFloor = newFloor;

        levelObjects.Clear();
        FindFloorObjects();


        // FIND OBSTACLES
        FindFloorObstacles();
    }
}
