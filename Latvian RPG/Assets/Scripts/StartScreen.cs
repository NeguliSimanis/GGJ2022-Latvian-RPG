using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{

    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject[] startSreenObjects;

    #region MAIN MENU
    [Header("Main menu")]
    [SerializeField]
    GameObject optionsCentralButton;
    #endregion

    #region CUTSCENES
    [Header("CUTSCENES")]
    [SerializeField]
    GameObject cutScene1;

    [SerializeField]
    GameObject cutScene2;

    [SerializeField]
    GameObject cutScene3;
    [SerializeField]
    Animator scene3Animator;

    float cutSceneDuration = 9f;

    bool scene1Active = false;
    bool scene2Active = false;
    bool scene3Active = false;
    bool cutscenesOver = false;
    #endregion

    #region CHAR SELECTION SCREEN
    [Header("char SELECTION")]
    [SerializeField]
    GameObject charSelectionScreen;

    [SerializeField]
    Transform charSelectionPanel;

    [SerializeField]
    GameObject charSelectButtonPrefab;

    [SerializeField]
    Sprite randomCharIcon;

    List<PlayerControls> characterList = new List<PlayerControls>();
    #endregion

    private void Start()
    {
#if UNITY_EDITOR
        if (GameData.current.isDebugMode)
            EndCutScenes();
#endif

        optionsCentralButton.SetActive(false);

        cutScene1.SetActive(false);
        cutScene2.SetActive(false);
        cutScene3.SetActive(false);
        charSelectionScreen.SetActive(false);
    }


    public void StartIntro()
    {
        if (RebirthManager.instance.HasRebirthBonus())
            SkipScene(skipToEnd: true);
        else
        StartCoroutine(PlayCutScene1());
    }

    private IEnumerator PlayCutScene1()
    {
        scene1Active = true;
        cutScene1.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        //scene1Animator.SetTrigger("fadeOut");
        StartCoroutine(PlayCutScene2());
    }

    private IEnumerator PlayCutScene2()
    {
        scene2Active = true;
        cutScene2.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        StartCoroutine(PlayCutScene3());

    }

    private IEnumerator PlayCutScene3()
    {
        scene3Active = true;
        cutScene3.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        scene3Animator.SetTrigger("fadeOut");
        cutScene2.SetActive(false);
        cutScene1.SetActive(false);
        foreach (GameObject gameObject in startSreenObjects)
        {
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1.4f);
        EndCutScenes();
    }

    private void EndCutScenes()
    {
        if (cutscenesOver)
            return;
        cutscenesOver = true;
        ShowCharSelection();
    }

    private void ShowCharSelection()
    {
        charSelectionScreen.SetActive(true);
        GetCharRosterData();

        // SPAWN AVAILABLE CHARACTERS
        foreach (PlayerControls newChar in characterList)
        {
            if (newChar.availableInRoster)
            {
                GameObject newButtonObj = Instantiate(charSelectButtonPrefab, charSelectionPanel);
                Button newButton = newButtonObj.GetComponent<Button>();
                newButton.onClick.AddListener((delegate { SelectCharacter(newChar.character); }));
                newButton.onClick.AddListener(gameManager.audioManager.PlayButtonSFX);
                DisplayCharData(newChar, newButtonObj);
            }
        }

        // SPAWN A RANDOM CHAR
        int charCount = characterList.Count;
        int randomRoll = Random.Range(0, charCount);
        PlayerControls randomChar = characterList[randomRoll];

        GameObject randomCharButtonObj = Instantiate(charSelectButtonPrefab, charSelectionPanel);
        Button randomCharButton = randomCharButtonObj.GetComponent<Button>();
        randomCharButton.onClick.AddListener((delegate { SelectCharacter(randomChar.character); }));
        randomCharButton.onClick.AddListener(gameManager.audioManager.PlayButtonSFX);
        DisplayCharData(randomChar, randomCharButtonObj, secretChar: true);
    }

    private void DisplayCharData(PlayerControls charToDisplay, GameObject displayPlace, bool secretChar = false)
    {
        SelectCharButton selectCharButton = displayPlace.GetComponent<SelectCharButton>();
        if (secretChar)
        {
            selectCharButton.charPortrait.sprite = randomCharIcon;
            selectCharButton.charName.text = "Random";
            selectCharButton.charSkills.text = "Starting skills: " +
                "????????" + ", " +
                "????????";
            return;
        }
        selectCharButton.charPortrait.sprite = charToDisplay.charPortrait;
        selectCharButton.charName.text = charToDisplay.name;
        selectCharButton.charSkills.text = "Starting skills: " +
            charToDisplay.startingSkills[0].skillName + ", " +
            charToDisplay.startingSkills[1].skillName;
    }

    /// <summary>
    /// Populates char list and returns list length
    /// </summary>
    /// <returns></returns>
    private void GetCharRosterData()
    {
        int rosterCharCount = 0;
        foreach (GameObject charObject in gameManager.charRoster)
        {
            PlayerControls newChar = charObject.GetComponent<PlayerControls>();
            characterList.Add(newChar);
            newChar.GetCharData();
            if (newChar.availableInRoster)
                rosterCharCount++;
        }
    }

    public void SelectCharacter(Character selectedChar)
    {
        gameManager.SpawnStartingChar(selectedChar, applyRebirthBonus: true);
        gameManager.StartGame();
        gameObject.SetActive(false);
    }

    public void SkipScene(bool skipToEnd = false)
    {
        if (scene3Active || skipToEnd)
        {
            StopCoroutine(PlayCutScene3());
            EndCutScenes();
        }
        else if (scene2Active)
        {
            StopCoroutine(PlayCutScene2());
            StartCoroutine(PlayCutScene3());
        }
        else if (scene1Active)
        {
            StopCoroutine(PlayCutScene1());
            StartCoroutine(PlayCutScene2());
        }
        
    }



    private void Update()
    {
        if (GameData.current.gameStarted)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipScene(skipToEnd: true);
            return;
        }
        if (Input.anyKeyDown)
        {
            SkipScene();
        }
        if (!scene1Active && Input.GetKeyDown(KeyCode.Return))
        {
            StartIntro();
        }
    }
}
