using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void SetButton(int index, string name, int mana, bool useHP, Sprite icon){
        magicSkillList[index].transform.Find("Image").GetComponent<Image>().sprite = icon;
        magicSkillList[index].transform.Find("skillName").GetComponent<TextMeshProUGUI>().text = name;
        magicSkillList[index].transform.Find("mp_bar").GetComponent<TextMeshProUGUI>().text = mana.ToString();
        // magicSkillList[index].transform.Find("Image").GetComponent<Image>().sprite = icon;
        // change Image source image using sprite icon


        
    }

    public void SwapBtn(){
        
    }


}
