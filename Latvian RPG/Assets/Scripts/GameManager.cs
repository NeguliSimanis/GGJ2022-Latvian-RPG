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
    Unknown,
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
    #region MANAGERS
    public static GameManager instance;
    public AudioManager audioManager;
    public PopupManager popupManager;
    public SaveLoad saveManager;
    public SkillManager skillManager;
    public CameraController cameraController;

    [SerializeField]
    private GameObject rebirthManagerObj;
    private TurnManager turnManager;
    #endregion


    #region UI
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

    int lastSelectedSkillButton = -1; // -1 - none , 0 - first , 1 - second
    #endregion 

    [SerializeField] GameObject targetHighlight;
    List<HighlightTileObject> skillHighlights = new List<HighlightTileObject>();

    #region CHAR MANAGEMENT
    public GameObject[] charRoster;
    public List<PlayerControls> allCharacters = new List<PlayerControls>(); // all characters currently in game, including NPCS
    public PlayerControls selectedChar;
    private PlayerControls lastSelectedPlayerChar;
    public PlayerControls highlightedChar;
    private bool isAnyCharSelected = false;
    public List<Vector2> allowedWalkCoordsNPC = new List<Vector2>();
    #endregion

    #region SKILLS
    public bool skillSelected = false;
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
    private GameObject[] endGameDungeonFloors;
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
        // REBIRTH
        if (GameObject.FindObjectOfType<RebirthManager>() == null)
        {
            GameObject newObj = Instantiate(rebirthManagerObj);
            RebirthManager.instance = newObj.GetComponent<RebirthManager>();
        }
        else
            RebirthManager.instance = GameObject.FindObjectOfType<RebirthManager>();


        if (GameData.current == null)
            GameData.current = new GameData();


        Debug.Log("CURR FLOOR " + GameData.current.dungeonFloor);
        Debug.Log("FLOOOORS CLEARED " + GameData.totalFloorsCleared);

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

    public void StartGame(bool isLoadedGame = false)
    {
        SpawnNewFloor(isLoadedProgress: isLoadedGame);

        GameData.current.gameStarted = true;
        GameData.current.PauseGame(false);
        GameData.current.playerTurnStartTime = Time.time;
        GameData.current.playerTurnEndTime = GameData.current.playerTurnStartTime + GameData.current.playerTurnTimer + 4f;
    }

    #region SPAWNING CHARACTERS
    private void SpawnLoadedCharacters()
    {
        foreach (CharacterStats loadedCharStats in saveManager.loadedCharStats)
        {
            if (!loadedCharStats.savedIsDead)
            {
                GameObject newCharacterObject = charRoster[0];

                // FIND THE SAVED CHARACTER IN THE ROSTER
                foreach (GameObject currChar in charRoster)
                {
                    if (currChar.GetComponent<PlayerControls>().character == loadedCharStats.savedCharacter)
                    {
                        newCharacterObject = currChar;
                    }
                }
                GameObject newPlayerInstance = Instantiate(newCharacterObject);
                PlayerControls newControls = newPlayerInstance.GetComponent<PlayerControls>();

                // LOAD CHAR ALIGNMENT
                newControls.charType = loadedCharStats.savedCharType;
                newControls.stats = loadedCharStats;
                allCharacters.Add(newControls);
                newControls.LoadSavedSkills(skillManager);

                if (newControls.charType == CharType.Player)
                {
                    cameraController.startTarget = newPlayerInstance.transform;
                    highlightedChar = newControls;
                    HighlightChar(newControls, highlight: true);

                    SelectChar(newControls);
                    cameraController.IntializeCamera(newPlayerInstance.transform, this, loadCamPos: true);
                }
                else
                {
                    NPC newNPC = newControls.gameObject.GetComponent<NPC>();
                    newNPC.npcControls = newControls;
                    newNPC.gameManager = this;
                }
                
                newControls.charMarker.UpdateMarkerColor(loadedCharStats.savedCharType);
                newControls.TeleportPlayerCharacter(loadedCharStats.lastSavedPosX, loadedCharStats.lastSavedPosY,
                    instantTeleport: true);
                StartCoroutine(newControls.AnimateStatNumbersForXSeconds(xSeconds: 0.1f,
                    charStat: CharStat.life,
                    animateAll: true));
            }
        }
        SelectChar(allCharacters[0]);
        HighlightChar(allCharacters[0], true);
    }

    public void SpawnRandomStartingChar()
    {
        int rosterLength = charRoster.Length;
        Character randomCharacter;
        int randomRoll = Random.Range(0, rosterLength);
        randomCharacter = charRoster[randomRoll].GetComponent<PlayerControls>().character;
        SpawnStartingChar(randomCharacter);
    }

    public void SpawnStartingChar(Character newCharacter, bool applyRebirthBonus = false)
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
        newControls.charType = CharType.Player;
        cameraController.startTarget = newPlayerInstance.transform;
        allCharacters.Add(newControls);

        highlightedChar = newControls;
        HighlightChar(newControls, highlight:true);
        SelectChar(newControls);
        if (GameData.totalFloorsCleared > 0 && applyRebirthBonus)
            RebirthManager.instance.ApplyRebirthBonus(newControls);
        cameraController.IntializeCamera(newPlayerInstance.transform, this);
        newControls.charMarker.UpdateMarkerColor(CharType.Player);
        MovePlayerToFloorStart();
    }
    #endregion

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
    }

    private void Start()
    {

        FindFloorObstacles();
        FindFloorObjects();

        // INITIALIZE BUTTONS
        endTurnButton.onClick.AddListener(EndTurn);
        //InitializeSkillButton();
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

    #region Interactable object management
    private void FindFloorObjects(bool isLoadedFloor = false)
    {
        levelObjects.Clear();
        foreach (InteractableObject interObject in FindObjectsOfType<InteractableObject>())
        {
            levelObjects.Add(interObject);
            if (isLoadedFloor)
            {
                foreach (string objName in saveManager.destroyedObjects)
                {
                    if (objName == interObject.name)
                    {
                        interObject.Disable();
                    }
                }
            }
            
        }
    }

    public void SaveDestroyedObjects()
    {
        List<string> destroyedObjNames = new List<string>();
        foreach (InteractableObject iObj in levelObjects)
        {
            if (iObj.consumed)
                destroyedObjNames.Add(iObj.name);
        }
        saveManager.destroyedObjects = destroyedObjNames;
    }

    public ObjectType CheckInteractableObject(Vector2 coord)
    {
        foreach (InteractableObject iObject in levelObjects)
        {
            if (MathUtils.FastApproximately(coord.x, iObject.xCoord, 0.1f) &&
                MathUtils.FastApproximately(coord.y,iObject.yCoord, 0.1f))
            {
                if (iObject.objType == ObjectType.HealingPotion && !iObject.consumed)
                {
                    iObject.Disable();
                    return ObjectType.HealingPotion;
                }
                else if (iObject.objType == ObjectType.LevelExit)
                {
                    return ObjectType.LevelExit;
                }
                else if (iObject.objType == ObjectType.LearnSkill && !iObject.consumed)
                {
                    iObject.Disable();
                    int i = iObject.GetComponent<Scholar>().SelectSkillsToTeach(selectedChar);
                    Debug.LogError("has to learn " + i + " skils0");
                    popupManager.DisplayScholarPopup(selectedChar);
                   
                    return ObjectType.LearnSkill;
                }
            }
        }
        return ObjectType.Undefined;
    }
    #endregion

    /// <summary>
    /// REMOVE CHARS THAT STAY IN OLD LEVEL
    /// </summary>
    public void RemoveOldNPCs()
    {
        int listLength = allCharacters.Count;
        for (int i = 0; i < listLength; i++)
        {
            if (allCharacters[i].charType != CharType.Player)
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

    public void MovePlayerToFloorStart(bool recoverMana = true)
    {
        DungeonFloor dungeonFloor = currFloor.GetComponent<DungeonFloor>();
        Transform levelStartPoint = dungeonFloor.levelStartPoint[0];
        Transform levelStartPoint2 = dungeonFloor.levelStartPoint[1];
        Transform levelStartPoint3 = dungeonFloor.levelStartPoint[2];
        Vector2 startingPoint = new Vector2(
            (levelStartPoint.position.x),
            (levelStartPoint.position.y));
        Vector2 startingPoint2 = new Vector2(
          (levelStartPoint2.position.x),
          (levelStartPoint2.position.y));
        Vector2 startingPoint3 = new Vector2(
          (levelStartPoint3.position.x),
          (levelStartPoint3.position.y));

        Debug.Log("start poi " + startingPoint);
        int playerID = 0;
        foreach (PlayerControls curPlayer in allCharacters)
        {
            if (curPlayer.charType == CharType.Player)
            {
                Debug.Log("TELEPORTING " + curPlayer.name + " to " + levelStartPoint.position);
                curPlayer.TeleportPlayerCharacter(levelStartPoint.position.x, levelStartPoint.position.y, instantTeleport: true);
                curPlayer.AddMana(amount: 0, addedBySkill: false, addToFull: true);
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
            switch (character.charType)
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

    public void SelectSkill(int skillID, bool select = true)
    {
        if (selectedChar.isMovingNow)
        {
            Debug.Log("moving, cant do this");
            return;
        }
        HideActionRange();

        // DETERMINE IF A BUTTON WAS SELECTED BEFORE

        if ((skillSelected && lastSelectedSkillButton == skillID) || !select)
        {
            skillSelected = false;
        }
        else
            skillSelected = true;
        lastSelectedSkillButton = skillID;

        popupManager.ColorSkillButts(popupManager.skillButts[0], isSelected: false,
            colorAll: true);

        // SKILL DESELECTED
        if (!skillSelected)
        {
            DisplayActionRange(ActionType.Walk);
            return;
        }

        // ATTEMPT TO USE MULTIPLE SKILLS PER TURN
        if (selectedChar.hasUsedSkillThisTurn)
        {
            popupManager.UpdateGuideText(selectedChar.name + " can use skill only once per turn!");
            DisplayActionRange(ActionType.Walk);
            return;
        }

        // SUCCESSFUL SKILL SELECT
        selectedSkill = selectedChar.currentSkills[skillID];
        popupManager.ColorSkillButts(popupManager.skillButts[skillID], isSelected: true,
           colorAll: false);
        if (selectedSkill.type[0] == SkillType.Damage)
        {
            DisplayActionRange();
        }
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
            popupManager.UpdateGuideText("Use " + selectedSkill.skillName);
            UnityEngine.Debug.Log("Use " + selectedSkill.skillName);
            actionRange = selectedSkill.skillRange;
        }
        else
        {
            int speedLeft = selectedChar.stats.speed - selectedChar.stats.tilesWalked;
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
        if ((actionType == ActionType.Walk && selectedChar.stats.tilesWalked < selectedChar.stats.speed)
            || (actionType == ActionType.UseUtilitySkill)
            || (actionType == ActionType.UseCombatSkill))
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
        if (IsTileOccupiedByObstacle(highlightLocation))
            return;
        //foreach (Obstacle obstacle in allObstacles)
        //{
        //    if (MathUtils.FastApproximately(obstacle.pos.x, highlightLocation.x, 0.2f)
        //         && MathUtils.FastApproximately(obstacle.pos.y, highlightLocation.y, 0.2f))
        //    {
                
        //        return;
        //    }
        //}


        // DONT SPAWN ON TOP OF CHAR IF THAT'S MOVE ACTION
        foreach (PlayerControls character in allCharacters)
        {
            if (actionType != ActionType.Walk)
                break;
            if (!character.isDead && 
                MathUtils.FastApproximately(character.xCoord, highlightLocation.x, 0.2f) &&
                MathUtils.FastApproximately(character.yCoord, highlightLocation.y, 0.2f))
                return;
        }

        allowedWalkCoordsNPC.Add(new Vector2(highlightLocation.x, highlightLocation.y));
        SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights, actionType);
    }

    public void HideActionRange()
    {

        //if (isAnyCharSelected)
        //    UpdateRemainingMovesText(selectedChar.playerSpeed - selectedChar.tilesWalked);
        if (skillSelected)
        {
            popupManager.ColorSkillButts(popupManager.skillButts[0], isSelected: false,
          colorAll: true);
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

        ListenForShortcuts();

    }

    private void ListenForShortcuts()
    {
        if (!isAnyCharSelected)
            return;
        // TURN MANAGEMENT
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EndTurn();
        }

        // POPUPS
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCharInfoPanel();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && popupManager.isWarningPopupActive)
        {
            popupManager.ShowWarningPopup(false);
        }

        // PAUSE
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameData.current.isGamePaused)
            {
                popupManager.ShowPausePanel(false);
            }
            else
            {
                popupManager.ShowPausePanel(true);
            }
        }

        // WALK SHORTCUTS
        //if (!skillSelected)
        //{
        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        ProcessInteractionRequest(selectedChar.xCoord, selectedChar.yCoord + 1, ActionType.Walk);
        //    } 
        //    if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        ProcessInteractionRequest(selectedChar.xCoord - 1, selectedChar.yCoord, ActionType.Walk);
        //    }
        //    if (Input.GetKeyDown(KeyCode.S))
        //    {
        //        ProcessInteractionRequest(selectedChar.xCoord, selectedChar.yCoord - 1, ActionType.Walk);
        //    }
        //    if (Input.GetKeyDown(KeyCode.D))
        //    {
        //        ProcessInteractionRequest(selectedChar.xCoord + 1, selectedChar.yCoord, ActionType.Walk);
        //    }
        //}



        // SKILL SHORTCUTS
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSkill(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSkill(1);
        }
        if (selectedChar.stats.skills.Count < 3)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSkill(2);
        }
        if (selectedChar.stats.skills.Count < 4)
            return;
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSkill(3);
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

        if (characterToSelect.charType == CharType.Player && GameData.current.turnType != CharType.Player)
            return characterSelected;
        foreach (PlayerControls character in allCharacters)
        {
            if (character == characterToSelect)
            {
                isAnyCharSelected = true;
                character.SelectCharacter(true);
                UpdateRemainingMovesText(character.stats.speed - character.stats.tilesWalked);
                characterSelected = true;
                selectedChar = characterToSelect;
                HideActionRange();
                if (character.charType != CharType.Player)
                {
                    return characterSelected;
                }
                lastSelectedPlayerChar = selectedChar;
                DisplayActionRange(ActionType.Walk, character.charType);
                ShowSkillButton();
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
        popupManager.DisplayCharSkillButts(selectedChar, display: show);

    }

    /// <summary>
    /// Returns true if there's any character in the player's party that can move or use ability
    /// </summary>
    /// <returns></returns>
    private bool CanPlayerPartyAct()
    {
        foreach (PlayerControls character in allCharacters)
        {
            if (character.charType == CharType.Player &&
                character.CanCharacterAct())
                return true;
        }
        return false;
    }

    private void MoveCameraToPlayer(bool instant = false)
    {
        cameraController.SetPosition(new Vector3(
            selectedChar.transform.position.x,
            selectedChar.transform.position.y + 1,
            cameraController.transform.position.z), instant);
    }

    public bool CheckForAlivePlayers()
    {
        foreach (PlayerControls character in allCharacters)
        {
            if (character.charType == CharType.Player && !character.isDead)
            {
                return true;
            }
        }
        return false;
    }

    private void LoseGame()
    {
        RebirthManager.instance.RollRebirthBonus();
        audioManager.PlayDefeatSFX();
        popupManager.DisplayRebirthPopup();
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
            if (character.charType == GameData.current.turnType)
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
            skillSelected = false;
            UpdateRemainingMovesText(selectedChar.stats.speed);
        }

        // MAKE NEUTRAL/ENEMY CHARS ACT
        if (GameData.current.turnType == CharType.Player)
            return;
        ShowTurnTimerBar(false);
        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacters[i].isDead)
            {
                // do nothin
            }
            else if ((allCharacters[i].charType == CharType.Neutral &&
                GameData.current.turnType == CharType.Neutral
                )
                ||
                (allCharacters[i].charType == CharType.Enemy &&
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
            if (character.charType == CharType.Player
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
            if (character.charType == CharType.Player
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
            if ((allCharacters[i].charType == CharType.Neutral &&
                GameData.current.turnType == CharType.Neutral)
                ||
                (allCharacters[i].charType == CharType.Enemy &&
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
    /// Attempt to use selected skill on X;Y coordinates. Can be called by clicking on neutral/enemy if skill selected and in range
    /// </summary>
    /// <param name="xCoordinate"></param>
    /// <param name="yCoordinate"></param>
    public void ProcessInteractionRequest(float xCoordinate, float yCoordinate, ActionType actionType)
    {
        if (GameData.current.turnType != CharType.Player)
            return;
        if (!GameData.current.gameStarted)
            return;
        if (selectedChar.isMovingNow)
            return;

        #region Unknown Action type
        // Determine action type and call method again if valid request
        if ((selectedChar.hasUsedSkillThisTurn || !skillSelected) && actionType == ActionType.Unknown)
        {
            return;
        }
        if (actionType == ActionType.Unknown)
        {
            if (!MathUtils.IsWithinDamageRange(
                target: new Vector2(xCoordinate, yCoordinate),
                damageSource: new Vector2(selectedChar.xCoord, selectedChar.yCoord),
                moveSpeed: 0,
                damageSkill: selectedSkill))
            {
                return;
            }
            switch (selectedSkill.type[0])
            {
                case SkillType.Buff:
                    ProcessInteractionRequest(xCoordinate, yCoordinate, ActionType.UseUtilitySkill);
                    return;
                case SkillType.Damage:
                    ProcessInteractionRequest(xCoordinate, yCoordinate, ActionType.UseCombatSkill);
                    return;
                case SkillType.Recruit:
                    ProcessInteractionRequest(xCoordinate, yCoordinate, ActionType.UseUtilitySkill);
                    return;
                default:
                    Debug.LogError("YO WTF DUDE");
                    return;
            }
        }
        #endregion

        // WALK INTERACTION
        if (actionType == ActionType.Walk)
        {

            // 1 - check if tile not occupied
            foreach (PlayerControls character in allCharacters)
            {
                if (MathUtils.FastApproximately(xCoordinate, character.xCoord, 0.01F) &&
                    MathUtils.FastApproximately(yCoordinate, character.yCoord, 0.01F) &&
                    !character.isDead)
                {
                    UnityEngine.Debug.Log("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            foreach (Obstacle obstacle in allObstacles)
            {
                if (MathUtils.FastApproximately(xCoordinate, obstacle.pos.x, 0.1F) &&
                    MathUtils.FastApproximately(yCoordinate, obstacle.pos.y, 0.1F))
                {
                    UnityEngine.Debug.LogError("TILE ALREADY OCCUPIED AT " + xCoordinate + ":" + yCoordinate);
                    popupManager.UpdateGuideText("Cannot move there!");
                    return;
                }
            }
            selectedChar.TeleportPlayerCharacter(xCoordinate, yCoordinate);
            UpdateRemainingMovesText(selectedChar.stats.speed - selectedChar.stats.tilesWalked);
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
                if (!character.isDead)
                {
                    UnityEngine.Debug.Log("TARGET ACQUIRED AT " + xCoordinate + ":" + yCoordinate);
                    target = character;
                    targetViable = true;
                }
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

        if (selectedChar.hasUsedSkillThisTurn == true)
        {
            popupManager.UpdateGuideText(selectedChar.stats.name + " has already used skill this turn!");
            return;
        }

        // 2.5 - hide current skill highlights
        // SelectSkill(0);
        HideActionRange();


        // 3 - apply skill effect
        if (selectedSkill.type[0] == SkillType.Damage)
        {
            if (selectedSkill.skillName == "Sap")
            {
                target.AddMana(-selectedSkill.skillDamage, addedBySkill: true, removeMana: true);
            }
            else
            {
                if (selectedSkill.skillName == "Wound")
                {
                    selectedSkill.ApplySkillEffects(target);
                }
                target.TakeDamage(-selectedSkill.skillDamage, selectedChar);
            }
            DisableSkillActions();
        }
        else if (selectedSkill.type[0] == SkillType.Recruit)
        {
            if (target.Convert(CharType.Player))
            {
                selectedChar.GainExp(ExpAction.Hire);
                DisableSkillActions();
            }
            }
        else if (selectedSkill.type[0] == SkillType.Buff)
        {
            selectedSkill.ApplySkillEffects(target);
            audioManager.PlayUtilitySFX();
            DisableSkillActions();
        }


        // 4 - remove spent mana
        // 5 - play animation
        GameObject newAnimation = Instantiate(selectedSkill.skillAnimation, target.transform);
        newAnimation.transform.SetParent(null);
        selectedChar.SpendMana(selectedSkill.manaCost);

    }

    public void DisableSkillActions (bool disable = true)
    {
        popupManager.HideUnusableButts(selectedChar, disable);
        selectedChar.hasUsedSkillThisTurn = disable;
        SelectSkill(-1, select: false);
    }

    public void RestartGame()
    {
        GameData.current = new GameData();
        SceneManager.LoadScene(0);
    }


    public bool IsTileAllowedForNPC(Vector2 coord)
    {
        foreach (Vector2 currCoord in allowedWalkCoordsNPC)
        {
            if (MathUtils.FastApproximately(currCoord.x, coord.x, 0.1f)
            && MathUtils.FastApproximately(currCoord.y, coord.y, 0.1f))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsTileOccupiedByObstacle(Vector2 coord)
    {
        foreach (Obstacle obstacle in allObstacles)
        {
            if (MathUtils.FastApproximately(obstacle.pos.x, coord.x, 0.1F)&&
                MathUtils.FastApproximately(obstacle.pos.y, coord.y, 0.1f))
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
                    if (character.charType == CharType.Neutral)
                        character.RegenMana();
                    break;
                case CharType.Enemy:
                    if (character.charType == CharType.Enemy)
                        character.RegenMana();
                    break;
                case CharType.Player:
                    if (character.charType == CharType.Player)
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

    public void SpawnNewFloor(bool isLoadedProgress = false)
    {
        if (GameData.current.dungeonFloor == -1)
        {
            return;
        }
        
        Debug.LogError("spawning floor. is saved previously?: " + isLoadedProgress);
        DestroyImmediate(currFloor);

        int newFloorID = GameData.current.dungeonFloor;

        // MANAGE MUSIC 
        audioManager.ManageMusicSwitch();

        bool spawnEndGame = false;
        
        if (newFloorID >= dungeonFloors.Length)
        {
            spawnEndGame = true;
            newFloorID = Random.Range(0, endGameDungeonFloors.Length);
        }


        GameObject newFloorObj;
        if (!spawnEndGame)
            newFloorObj = Instantiate(dungeonFloors[newFloorID]);
        else
            newFloorObj = Instantiate(endGameDungeonFloors[newFloorID]);
        currFloor = newFloorObj;
        DungeonFloor newFloor = newFloorObj.GetComponent<DungeonFloor>();
        newFloor.InitializeFloor(spawnEnemies: !isLoadedProgress);

        levelObjects.Clear();
        popupManager.UpdateFloorText();
        FindFloorObjects(isLoadedProgress);


        // FIND OBSTACLES
        FindFloorObstacles();
    }

    public void VictoryCheck(ExpAction expAction)
    {
        if (selectedChar.stats.UpdateProgressToGameVictory(expAction))
        {
            Victory();
        }
    }

    public void PauseGame(bool pause)
    {
        GameData.current.PauseGame(pause);
    }

    public IEnumerator PauseGameAfterSeconds(bool pause, float seconds = 0f, bool debug = false)
    {
        yield return new WaitForSeconds(seconds);

        PauseGame(pause);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveAndExitToMenu()
    {
        saveManager.SaveGame(this);
        RestartGame();
    }

    public void LoadGame()
    {
        popupManager.startScreen.SetActive(false);
        saveManager.LoadGame(this);
        SpawnLoadedCharacters();
        StartGame(isLoadedGame: true);

        //MovePlayerToFloorStartingPoint();
    }
   
}
