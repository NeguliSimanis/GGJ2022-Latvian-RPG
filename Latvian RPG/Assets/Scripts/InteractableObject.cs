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
    public float xCoord;
    public float yCoord;
    public bool consumed = false;
    public ObjectType objType = ObjectType.HealingPotion;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    void Start()
    {
        xCoord = transform.position.x;
        yCoord = transform.position.y;
        spriteRenderer.sortingOrder = -(int)yCoord;
    }

    public void Disable()
    {
        consumed = true;
        gameObject.SetActive(false);
    }
}
