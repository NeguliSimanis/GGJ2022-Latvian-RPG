using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer obstacleSpriteRenderer;
    [SerializeField]
    Transform obstacleTransform;

    public Vector2 pos = new Vector2();


    private void Start()
    {
        obstacleSpriteRenderer.sortingOrder = GameData.current.charactersSortingOrder -
            (int)transform.position.y - 1;
        GetPos();
    }


    public void GetPos()
    {
        pos = new Vector2(obstacleTransform.position.x, obstacleTransform.position.y);
    }
}
