using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes marker color depending on whether char is enemy/friend/neutral
/// </summary>
public class CharacterMarker : MonoBehaviour
{

    private bool animateMarker = false;
    private float rotationSpeed = 0.25f;
    [SerializeField]
    Color playerColor;
    [SerializeField]
    Color neutralColor;
    [SerializeField]
    Color enemyColor;

    [SerializeField]
    SpriteRenderer markerImage;

    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    Sprite highlightSprite;

    Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void UpdateMarkerColor(CharType charType)
    {
        switch (charType)
        {
            case CharType.Enemy:
                markerImage.color = enemyColor;
                break;
            case CharType.Player:
                markerImage.color = playerColor;
                break;
            case CharType.Neutral:
                markerImage.color = neutralColor;
                break;
        }
    }

    public void UpdateSortingOrder(SpriteRenderer parentSprite)
    {
        markerImage.sortingOrder = parentSprite.sortingOrder - 1;
    }

    public void AnimateMarker(bool animate = true)
    {
        animateMarker = animate;
        //animator.SetBool("spin", animate);
        if (animate)
        {
            markerImage.sprite = highlightSprite;
            
        }
        else
        {
            //ResetRotation();
            markerImage.sprite = defaultSprite;
        }
    }

    private void ResetRotation()
    {
        Debug.Log("reset rot " + transform.rotation);
        float offset = 360f - transform.rotation.z;
        Debug.Log("oofset " + offset);
        transform.Rotate(0, 0, +offset, Space.Self);
    }

    private void Update()
    {
        if (animateMarker)
        {
            transform.Rotate(0, 0, rotationSpeed, Space.Self);
        }
        else if (transform.rotation.z > 0)
        {
            transform.Rotate(0, 0, -rotationSpeed*8, Space.Self);
        }
        //transform.Rotate(new Vector3(0,0,rotationSpeed), Space.Self);
        //transform.eulerAngles = Vector3.forward * 50;
    }
}
