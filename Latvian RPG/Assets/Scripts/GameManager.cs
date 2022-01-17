using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void ShowTile(bool show = true)
    {
        if (show)
        {
            spriteRenderer.color = red;
        }
        else
        {
            spriteRenderer.color = transparent;
        }
        tileHighlight.ShowTile(show);
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
    Text remainingMovesText;
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
        remainingMovesText.text = "";
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
        if (skillSelected)
        {
            selectedSkill = selectedCharacter.stats.skills[0];
            DisplaySkillRange();

        }
        else
        {
            HideSkillRange();
        }
    }

    private void DisplaySkillRange()
    {
        remainingMovesText.text = "Select Target";
        skillButtonImage.color = skillButtonSelectedColor;
        bool reuseOldTargetHighlights = false;
        targetHighlightCounter = 0;
        if (skillHighlights.Count > 0)
        {
            reuseOldTargetHighlights = true;
        }


        for (int i = 1; i < selectedSkill.skillRange + 1; i++)
        {
            // spawn up
            Vector3 highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);



            // spawn up-right && up-left
            for (int z = 1; z < selectedSkill.skillRange - i + 1; z++)
            {
                highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);

                highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);
            }

            //spawn down
            highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);



            // spawn down-right && down-left
            for (int z = 1; z < selectedSkill.skillRange - i + 1; z++)
            {
                highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);

                highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);
            }

            // spawn right
            highlightLocation = new Vector3(selectedCharacter.transform.position.x + i,
                selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);

            // spawn left
            highlightLocation = new Vector3(selectedCharacter.transform.position.x - i,
                selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
            SpawnHighlightTile(highlightLocation, reuseOldTargetHighlights);
        }
    }

    private void HideSkillRange()
    {
        UpdateRemainingMovesText(selectedCharacter.playerSpeed - selectedCharacter.tilesWalked);
        skillButtonImage.color = skillButtonDefaultColor;

        foreach (HighlightTileObject skillHighlight in skillHighlights)
        {
            skillHighlight.ShowTile(false);
            // SetActive(false);
        }
    }

    private void SpawnHighlightTile(Vector3 highLightLocation, bool oldHighlightsExist)
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
                skillHighlights.Add(new HighlightTileObject(targetHighlightCounter,newHighlight));
                
                break;


            case false:
                skillHighlights[targetHighlightCounter-1].ShowTile();
                skillHighlights[targetHighlightCounter-1].highlightObject.transform.position = highLightLocation;
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
        remainingMovesText.text = "Remaining moves: " + (remainingMoves-1).ToString();
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
        
        foreach(PlayerControls character in allCharacters)
        {
            character.tilesWalked = 0;
        }
        currentTurnText.text = "Turn " + GameData.current.currentTurn.ToString();
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
    public void ProcessSkillUseRequest(int xCoordinate, int yCoordinate)
    {
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
            Debug.Log("NOOO TARGET AT " + xCoordinate + ":" + yCoordinate);
            return;
        }
        // 2 - check if enough mana


        // 2.5 - hide current skill highlights
        SelectSkill();


        // 3 - apply skill effect
        target.TakeDamage(selectedSkill.skillDamage);

    }
}
