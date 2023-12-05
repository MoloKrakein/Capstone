using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicSkillList : MonoBehaviour
{
    // list of a button
    public Button[] magicSkillList;
    public GameObject btnPrefab;
    public Button backButton;

    public GameObject magicSkillPanel;

    public int  ButtonAction(int index)
    {
        return index;
    }

    public void HideButton(){
        magicSkillPanel.SetActive(false);
    }

    public void ShowButton(){
        magicSkillPanel.SetActive(true);
    }

    public void SetButton(int index, string name, Sprite icon){
        magicSkillList[index].GetComponentInChildren<Text>().text = name;
        magicSkillList[index].GetComponentInChildren<Image>().sprite = icon;
    }

    public void SwapBtn(){
        
    }


}
