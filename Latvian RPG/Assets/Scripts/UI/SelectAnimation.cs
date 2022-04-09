using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject upMarker;
    [SerializeField]
    bool showUpMarker = true;

    private void Start()
    {
        //if (!showUpMarker)
        //    upMarker.GetComponent<SpriteRenderer>().enabled = false;
    }
}
