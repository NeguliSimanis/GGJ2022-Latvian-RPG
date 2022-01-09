using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    List<PlayerControls> playerCharacters = new List<PlayerControls>();
    [SerializeField]
    GameObject targetHighlight;
    List<GameObject> targetHighlights = new List<GameObject>();

    public PlayerControls selectedCharacter;


    #region SKILLS
    bool skillSelected = false;
    Image skillButtonImage;
    Color skillDefaultColor = Color.white;
    Color skillSelectedColor = Color.red;
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
            playerCharacters.Add(character);
        }
        endTurnButton.onClick.AddListener(EndTurn);
        InitializeAbilityButton();
        remainingMovesText.text = "";
    }

    private void InitializeAbilityButton()
    {
        skillButton.onClick.AddListener(DisplaySkillRange);
        skillButtonText = skillButton.transform.GetChild(0).GetComponent<Text>();
        skillButtonImage = skillButton.gameObject.GetComponent<Image>();
        skillButton.gameObject.SetActive(false);
    }

    private void DisplaySkillRange()
    {
        skillSelected = !skillSelected;
        if (skillSelected)
        {
            remainingMovesText.text = "Select Target";
            skillButtonImage.color = skillSelectedColor;

            Skill selectedSkill = selectedCharacter.stats.skills[0];
            int topSkillRows = selectedSkill.skillRange;
            int bottomSkillRows = selectedSkill.skillRange;
            int rightRows = selectedSkill.skillRange;
            int leftRows = selectedSkill.skillRange;

            for (int i = 1; i < selectedSkill.skillRange+1; i++)
            {
                GameObject newHighlight = targetHighlight;
                // spawn up
                Vector3 highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                    selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(newHighlight, highlightLocation);
                targetHighlights.Add(newHighlight);

                // spawn up-right && up-left
                for (int z = 1; z < selectedSkill.skillRange - i + 1; z++)
                {
                    highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                    selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                    SpawnHighlightTile(newHighlight, highlightLocation);
                    targetHighlights.Add(newHighlight);

                    highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                    selectedCharacter.transform.position.y + i, selectedCharacter.transform.position.z);
                    SpawnHighlightTile(newHighlight, highlightLocation);
                    targetHighlights.Add(newHighlight);
                }

                //spawn down
                highlightLocation = new Vector3(selectedCharacter.transform.position.x,
                    selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                SpawnHighlightTile(newHighlight, highlightLocation);
                targetHighlights.Add(newHighlight);


                // spawn down-right && down-left
                for (int z = 1; z < selectedSkill.skillRange - i + 1; z++)
                {
                    highlightLocation = new Vector3(selectedCharacter.transform.position.x - z,
                    selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                    SpawnHighlightTile(newHighlight, highlightLocation);
                    targetHighlights.Add(newHighlight);

                    highlightLocation = new Vector3(selectedCharacter.transform.position.x + z,
                    selectedCharacter.transform.position.y - i, selectedCharacter.transform.position.z);
                    SpawnHighlightTile(newHighlight, highlightLocation);
                    targetHighlights.Add(newHighlight);
                }

                // spawn right
                highlightLocation = new Vector3(selectedCharacter.transform.position.x + i,
                    selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
                SpawnHighlightTile(newHighlight, highlightLocation);
                targetHighlights.Add(newHighlight);

                // spawn left
                highlightLocation = new Vector3(selectedCharacter.transform.position.x - i,
                    selectedCharacter.transform.position.y, selectedCharacter.transform.position.z);
                SpawnHighlightTile(newHighlight, highlightLocation);
                targetHighlights.Add(newHighlight);
            }

           
        }
        // hide skill range
        else
        {
            UpdateRemainingMovesText(selectedCharacter.playerSpeed - selectedCharacter.tilesWalked);
            skillButtonImage.color = skillDefaultColor;
            foreach (GameObject skillHighlight in targetHighlights)
            {
                Debug.Log("fuck you " + skillHighlight.transform.position);
                skillHighlight.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0); //SetActive(false);
            }
        }
    }

    private void SpawnHighlightTile(GameObject newHighlight, Vector3 highLightLocation)
    {
        Instantiate(newHighlight, highLightLocation, Quaternion.identity);
        newHighlight.GetComponent<SpriteRenderer>().color = new Color(1, 0.05f, 0.05f, 0.4666667f);
        //newHighlight.SetActive(true);
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
        remainingMovesText.text = "Remaining moves: " + remainingMoves.ToString();
    }

    /// <summary>
    /// Returns true if character selected successfully
    /// </summary>
    /// <param name="characterToSelect"></param>
    /// <returns></returns>
    public bool SelectCharacter(PlayerControls characterToSelect)
    {
        bool characterSelected = false;
        foreach (PlayerControls character in playerCharacters)
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
        
        foreach(PlayerControls character in playerCharacters)
        {
            character.tilesWalked = 0;
        }
        currentTurnText.text = "Turn " + GameData.current.currentTurn.ToString();
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
}
