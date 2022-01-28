using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    public int xCoord;
    public int yCoord;

    void Start()
    {
        xCoord = (int)transform.position.x;    
        yCoord = (int)transform.position.y;    
    }
}
