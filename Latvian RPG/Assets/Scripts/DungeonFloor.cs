using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonFloor : MonoBehaviour
{
    [SerializeField]
    private GameObject [] npcRooster;
    private GameManager gameManager;

    public Transform levelStartPoint;
    public Transform[] spawnPoints;

    int spawnPointCount;
    int npcCount;
    bool repeatNPCs = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        SpawnNPCs();
    }
    private void SpawnNPCs()
    {
        spawnPointCount = spawnPoints.Length;
        npcCount = npcRooster.Length;
        repeatNPCs = false;

        if (npcCount < spawnPointCount)
            repeatNPCs = true;

        for (int i = 0; i < spawnPointCount; i++)
        {
            SpawnPoint spawnPoint = spawnPoints[i].GetComponent<SpawnPoint>();
            if (spawnPoint.preferredSpawn == Character.Undefined)
                SpawnRandomNPC(spawnPoints[i]);
            else
            {
                Character charToSpawn;
                switch (spawnPoint.preferredSpawn)
                {
                    case Character.Crow:
                        charToSpawn = Character.Crow;
                        break;
                    case Character.Luna:
                        charToSpawn = Character.Luna;
                        break;
                    case Character.Dog:
                        charToSpawn = Character.Dog;
                        break;
                    case Character.Goat:
                        charToSpawn = Character.Goat;
                        break;
                    case Character.Wolf:
                        charToSpawn = Character.Wolf;
                        break;
                    default:
                        Debug.Log("PROBLEM HERE!");
                        charToSpawn = Character.Undefined;
                        break;
                }
                SpawnSpecificNPC(spawnPoints[i], charToSpawn);
            }
        }
        if (repeatNPCs)
        {
            
        }
        else
        {

        }
    }

    private void SpawnSpecificNPC(Transform spawnTransform, Character character)
    {
        foreach (GameObject npcObject in npcRooster)
        {
            if (npcObject.GetComponent<PlayerControls>().character == character)
            {
                GameObject newNPC = Instantiate(npcObject, spawnTransform);
                PlayerControls newNPCControls = newNPC.GetComponent<PlayerControls>();

                gameManager.AddNewCharacter(newNPCControls);
                return;
            }
        }
    }

    private void SpawnRandomNPC(Transform spawnTransform)
    {
        int npcRoll = Random.Range(0, npcCount);
        GameObject newNPC = Instantiate(npcRooster[npcRoll], spawnTransform);
        PlayerControls newNPCControls = newNPC.GetComponent<PlayerControls>();

        gameManager.AddNewCharacter(newNPCControls);
    }
}
