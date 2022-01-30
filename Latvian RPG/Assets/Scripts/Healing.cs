﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public int xCoord;
    public int yCoord;
    public bool consumed = false;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    void Start()
    {
        xCoord = (int)transform.position.x;
        yCoord = (int)transform.position.y;
        spriteRenderer.sortingOrder = -yCoord;
    }
}
