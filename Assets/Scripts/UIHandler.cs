using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public GameObject[] UIs;
    public GameObject[] UIsToHide;
    public GameObject[] UIsToShow;

    public void ShowUI()
    {
        foreach (GameObject UI in UIsToShow)
        {
            UI.SetActive(true);
        }
    }

    public void HideUI()
    {
        foreach (GameObject UI in UIsToHide)
        {
            UI.SetActive(false);
        }
    }

    public void ToggleUI()
    {
        foreach (GameObject UI in UIs)
        {
            UI.SetActive(!UI.activeSelf);
        }
    }
}
