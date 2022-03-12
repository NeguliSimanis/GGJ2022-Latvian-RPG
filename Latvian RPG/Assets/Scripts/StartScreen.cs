using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject[] startSreenObjects;

    [SerializeField]
    GameObject cutScene1;
    [SerializeField]
    Animator scene1Animator;

    [SerializeField]
    GameObject cutScene2;

    [SerializeField]
    GameObject cutScene3;
    [SerializeField]
    Animator scene3Animator;

    float cutSceneDuration = 9f;

    bool scene1Active = false;
    bool scene2Active = false;
    bool scene3Active = false;

    private void Start()
    {
#if UNITY_EDITOR
        if (GameData.current.isDebugMode)
            EndCutScenes();
        #endif
        cutScene1.SetActive(false);
        cutScene2.SetActive(false);
        cutScene3.SetActive(false);
    }


    public void StartIntro()
    {
        StartCoroutine(PlayCutScene1());
    }

    private IEnumerator PlayCutScene1()
    {
        scene1Active = true;
        cutScene1.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        //scene1Animator.SetTrigger("fadeOut");
        StartCoroutine(PlayCutScene2());
    }

    private IEnumerator PlayCutScene2()
    {
        scene2Active = true;
        cutScene2.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        StartCoroutine(PlayCutScene3());

    }

    private IEnumerator PlayCutScene3()
    {
        scene3Active = true;
        cutScene3.SetActive(true);
        yield return new WaitForSeconds(cutSceneDuration);
        scene3Animator.SetTrigger("fadeOut");
        cutScene2.SetActive(false);
        cutScene1.SetActive(false);
        foreach (GameObject gameObject in startSreenObjects)
        {
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1.4f);
        EndCutScenes();
    }

    private void EndCutScenes()
    {
        gameManager.StartGame();
        gameObject.SetActive(false);
    }

    private void SkipScene(bool skipToEnd = false)
    {
        if (scene3Active || skipToEnd)
        {
            StopCoroutine(PlayCutScene3());
            EndCutScenes();
        }
        else if (scene2Active)
        {
            StopCoroutine(PlayCutScene2());
            StartCoroutine(PlayCutScene3());
        }
        else if (scene1Active)
        {
            StopCoroutine(PlayCutScene1());
            StartCoroutine(PlayCutScene2());
        }
        
    }



    private void Update()
    {
        if (GameData.current.gameStarted)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipScene(skipToEnd: true);
            return;
        }
        if (Input.anyKeyDown)
        {
            SkipScene();
        }
        if (!scene1Active && Input.GetKeyDown(KeyCode.Return))
        {
            StartIntro();
        }
    }
}
