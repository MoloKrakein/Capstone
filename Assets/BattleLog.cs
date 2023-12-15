using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : MonoBehaviour
{
    public TextMeshProUGUI logText;

    public void UpdateLog(string text)
    {
        logText.text = text;
    }
}



