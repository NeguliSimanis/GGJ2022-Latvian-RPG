using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public List<CharacterStats> loadedCharStats = new List<CharacterStats>();

    public float loadedCamPosX;
    public float loadedCamPosY;
    public float loadedCamPosZ;

    public List<string> destroyedObjects = new List<string>();

    public void SaveGame(GameManager gameManager)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");

        #region SAVE CHAR STATS
        List<CharacterStats> charsToSave = new List<CharacterStats>();
        foreach (PlayerControls character in gameManager.allCharacters)
        {
            character.stats.savedCharType = character.charType;
            character.stats.lastSavedPosX = character.xCoord;
            character.stats.lastSavedPosY = character.yCoord;
            character.stats.savedCharacter = character.character;
            character.stats.savedIsDead = character.isDead;
            charsToSave.Add(character.stats);
        }
        #endregion

        gameManager.SaveDestroyedObjects();

        SaveData data = new SaveData(
            dungeonFloor: GameData.current.dungeonFloor,
            characterStats: charsToSave,
            gameManager: gameManager,
            desObjNames: destroyedObjects,
            gameDataToSave: GameData.current);


        data.floorReached = GameData.current.dungeonFloor;

        //
        bf.Serialize(file, data);
        file.Close();

        Debug.LogError("Game data saved! REAched - " + GameData.current.RealFloor());
    }


    public void LoadGame(GameManager gameManager)
    {
        if (File.Exists(Application.persistentDataPath
                       + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/MySaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            // general
            GameData.current = data.savedGameData;
           
            // ENVIRONMENT
            GameData.current.dungeonFloor = data.floorReached;
            destroyedObjects = data.destroyedObjNames;

            // CHARACTERS
            loadedCharStats = data.charStats;
            //gameManager.allCharacters = data.activeCharacters;

            // CAMERA
            loadedCamPosX = data.cameraPosX;
            loadedCamPosY = data.cameraPosY;
            loadedCamPosZ = data.cameraPosZ;

            Debug.LogError("Game data loaded! Floor reached - " + GameData.current.RealFloor());

        }
        else
            Debug.LogError("There is no save data!");
    }

    public bool SaveAvailable()
    {
        if (File.Exists(Application.persistentDataPath
                      + "/MySaveData.dat"))
        {
            return true;
        }
        else
            return false;
    }
}


[Serializable]
class SaveData
{

    public List<CharacterStats> charStats = new List<CharacterStats>();

    public GameData savedGameData;
    public int floorReached;
    public List<string> destroyedObjNames = new List<string>();

    public float cameraPosX;
    public float cameraPosY;
    public float cameraPosZ;

    public SaveData(
        List<CharacterStats> characterStats,
        int dungeonFloor,
        GameManager gameManager,
        List<string> desObjNames,
        GameData gameDataToSave)
    {

        floorReached = dungeonFloor;
        charStats = characterStats;

        cameraPosX = gameManager.cameraController.transform.position.x;
        cameraPosY = gameManager.cameraController.transform.position.y;
        cameraPosZ = gameManager.cameraController.transform.position.z;

        destroyedObjNames = desObjNames;

        savedGameData = gameDataToSave;
    }
}
