using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad : MonoBehaviour
{
    public void SaveGame(GameManager gameManager)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/MySaveData.dat");
        SaveData data = new SaveData(
            dungeonFloor: GameData.current.dungeonFloor,
            characters: gameManager.allCharacters);


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
            //gameManager.allCharacters = data.activeCharacters;

            Debug.LogError("Game data loaded! Floor reached - " + GameData.current.RealFloor());
            foreach (PlayerControls player in gameManager.allCharacters)
                Debug.LogError(player.stats.name);
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
     */
    //public List<PlayerControls> activeCharacters = new List<PlayerControls>();

    /* 
     * FLOOR
     *  - collectibles 
     * 
     */
    public int floorReached;

    public SaveData(
        List<PlayerControls> characters,
        int dungeonFloor)
    {
        floorReached = dungeonFloor;
        //activeCharacters = characters;
    }
}
