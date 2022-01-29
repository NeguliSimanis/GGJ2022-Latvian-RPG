using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    private PlayerControls parentControls;
    private PopupManager popupManager;

    private void Start()
    {
        parentControls = transform.parent.gameObject.GetComponent<PlayerControls>();
        popupManager = FindObjectOfType<PopupManager>().GetComponent<PopupManager>();
    }
    private void OnMouseDown()
    {
        popupManager.ShowCharPopup(parentControls, true);
    }
}
