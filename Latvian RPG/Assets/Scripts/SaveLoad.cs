using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public List<CharacterStats> loadedCharStats = new List<CharacterStats>();

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
            charsToSave.Add(character.stats);
        }
        #endregion

        SaveData data = new SaveData(
            dungeonFloor: GameData.current.dungeonFloor,
            characterStats: charsToSave);


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

           

            GameData.current.dungeonFloor = data.floorReached;
            loadedCharStats = data.charStats;
            //gameManager.allCharacters = data.activeCharacters;

            Debug.LogError("Game data loaded! Floor reached - " + GameData.current.RealFloor());
            foreach (CharacterStats player in loadedCharStats)
                Debug.LogError(player.name);

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
    /* 
     * CHARACTERS
     * - SKILLS
     * - MANA, LIFE, XP, SPEED
     * - POSITION
     * - STATUS EFFECTS
     * - enemy / neutal / player
     */
    public List<CharacterStats> charStats = new List<CharacterStats>();

    /* 
     * FLOOR
     *  - collectibles 
     * 
     */
    public int floorReached;

    public SaveData(
        List<CharacterStats> characterStats,
        int dungeonFloor)
    {
        floorReached = dungeonFloor;
        charStats = characterStats;
        //activeCharacters = characters;
    }
}
