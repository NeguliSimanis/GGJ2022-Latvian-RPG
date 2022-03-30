using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimation : MonoBehaviour
{

    private float yOffset = 0.6f;
    [SerializeField]
    AnimationClip thisAnimation;
    float lifeTime;
    float deathTime;


    void Start()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + yOffset);
        lifeTime = thisAnimation.length;
        deathTime = Time.time + lifeTime;
    }


    private void Update()
    {
        if (Time.time > deathTime)
            Destroy(gameObject);
    }
}
