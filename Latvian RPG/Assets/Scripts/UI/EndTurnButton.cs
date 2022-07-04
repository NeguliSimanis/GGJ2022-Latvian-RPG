using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField]
    Text buttonText;

    private void Start()
    {
        buttonText.text = "End Turn\n(Enter)";
#if UNITY_ANDROID
        buttonText.text = "End Turn";
#endif
    }
}
