using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject charInfoPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCharInfoPanel();
        }

    }

    /// <summary>
    /// Check whether panel was opened by mouse so that you don't accidentally close it with mouse click
    /// </summary>
    /// <param name="toggledByMouse"></param>
    public void ToggleCharInfoPanel(bool toggledByMouse = false)
    {
        if (!toggledByMouse)
            charInfoPanel.SetActive(!charInfoPanel.activeInHierarchy);
        else
            charInfoPanel.SetActive(true);
    }
}
