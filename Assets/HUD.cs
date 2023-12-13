using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using textmeshpro;
using TMPro;

public class HUD : MonoBehaviour
{
    // public TextMeshProUGUI nameText;
    // public TextMeshProUGUI lvlText;
    // public TextMeshProUGUI dmgText;

    // public GameObject hpBar;
    // public GameObject mpBar;

    public Slider hpSlider;
    public Slider mpSlider;

    public GameObject HUDObject;

    public void setupHUD(Unit unit)
    {

        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        mpSlider.maxValue = unit.maxMP;
        mpSlider.value = unit.currentMP;

        // if(hpSlider.value > hpSlider.maxValue)
        // {
        //     hpSlider.maxValue = hpSlider.value;
        // }
    }
    // update damage text
    public void setupEnemyHPHUD(Unit unit)
    {

        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        mpSlider.maxValue = unit.maxMP;
        mpSlider.value = unit.currentMP;
    }
    public void updateHP(int hp)
    {
        hpSlider.value = hp;
    }
    // update mana text
    public void updateMP(int mp)
    {
        mpSlider.value = mp;
    }

    public void hideUI()
    {
        // originalTransform = HUDObject.transform;
        HUDObject.transform.position = new Vector3(HUDObject.transform.position.x + 1000, HUDObject.transform.position.y, HUDObject.transform.position.z);

    }

    public void showUI()
    {
        HUDObject.transform.position = new Vector3(HUDObject.transform.position.x - 1000, HUDObject.transform.position.y, HUDObject.transform.position.z);

    }
}