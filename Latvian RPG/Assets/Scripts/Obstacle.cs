using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer obstacleSpriteRenderer;
    [SerializeField]
    Transform obstacleTransform;

    public Vector2Int pos = new Vector2Int();


    private void Start()
    {
        obstacleSpriteRenderer.sortingOrder = GameData.current.charactersSortingOrder -
            (int)transform.position.y - 1;
        GetPos();
    }


    public void GetPos()
    {
        pos = new Vector2Int((int)obstacleTransform.position.x, (int)obstacleTransform.position.y);
    }
}
