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
    public Slider hpSlider;

    public void setupHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        lvlText.text = "Lvl " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void updateHP(int hp)
    {
        hpSlider.value = hp;
    }
}
