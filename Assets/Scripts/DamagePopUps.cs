using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopUps : MonoBehaviour
{
    public Slider HealthBar;
    public TextMeshProUGUI dmgValue1;
    public TextMeshProUGUI dmgValue2;
    public float LifeTime;

    public Transform EnemyPopUpLocation;
    public Transform PlayerPopUpLocation;

    public GameObject DownImage;

    public void SetupDmgPopup(int maxHealth){
        HealthBar.maxValue = maxHealth;

    }

    public void UpdateHealthBar(int health)
    {
        HealthBar.value = health;
        // HealthBar.maxValue = maxHealth;
        // HealthBar.maxValue = 100;
        // HealthBar.minValue = 0;

    }

    void dmgValue1Text(int damage)
    {
        dmgValue1.text = damage.ToString();
    }

    void dmgValue2Text(int damage)
    {
        dmgValue2.text = damage.ToString();
    }

    public void spawnPopups(int damage, bool isPlayer, bool isDown, int Health){
        if(isDown){
            DownImage.SetActive(true);
        }
        else{
            DownImage.SetActive(false);
        }
        if(isPlayer){
            transform.position = PlayerPopUpLocation.position;
        }
        else{
            transform.position = EnemyPopUpLocation.position;
        }
        // Health = Health - damage;
        dmgValue1Text(damage);
        dmgValue2Text(damage);
        UpdateHealthBar(Health);
        StartCoroutine(DestroyPopups());
    }

    IEnumerator DestroyPopups(){
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }


}
