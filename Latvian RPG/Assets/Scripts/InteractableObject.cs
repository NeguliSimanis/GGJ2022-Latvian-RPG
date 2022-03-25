using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    HealingPotion,
    LevelExit,
    LearnSkill,
    ForgetSkill,
    Undefined
}

public class InteractableObject : MonoBehaviour
{
    public int xCoord;
    public int yCoord;
    public bool consumed = false;
    public ObjectType objType = ObjectType.HealingPotion;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    void Start()
    {
        xCoord = (int)transform.position.x;
        yCoord = (int)transform.position.y;
        spriteRenderer.sortingOrder = -yCoord;
    }
}
