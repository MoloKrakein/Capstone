using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using textmeshpro;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI lvlText;
    public TextMeshProUGUI dmgText;
    public Slider hpSlider;
    public Slider mpSlider;
    

    public void setupHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        lvlText.text = "Lvl " + unit.unitLevel;
        dmgText.text = "Damage: " + unit.damage;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        mpSlider.maxValue = unit.maxMP;
        mpSlider.value = unit.currentMP;
    }
    // update damage text
    public void setupEnemyHPHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        lvlText.text = "Lvl " + unit.unitLevel;
        dmgText.text = "Damage: " + unit.damage;
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
}