using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class customSlider : MonoBehaviour
{
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI MPText;

    public Slider HPBar;
    public Slider MPBar;

    public void setupMax(int maxHP, int maxMP)
    {
        HPBar.maxValue = maxHP;
        MPBar.maxValue = maxMP;
    }
    void Start()
    {
        HPText.text = "HP: " + HPBar.value + "/" + HPBar.maxValue;
        MPText.text = "MP: " + MPBar.value + "/" + MPBar.maxValue;
    }

    public void updateHP(int hp)
    {
        HPText.text = "HP: " + hp + "/" + HPBar.maxValue;
    }

    public void updateMP(int mp)
    {
        MPText.text = "MP: " + mp + "/" + MPBar.maxValue;
    }

    public void updateHPBar(int hp)
    {
        HPBar.value = hp;
    }

    public void updateMPBar(int mp)
    {
        MPBar.value = mp;
    }

    void Update()
    {
        HPText.text = HPBar.value + "";
        MPText.text = MPBar.value + "";
    }
    

    

}
