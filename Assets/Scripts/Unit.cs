using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;

    public bool TakeDamage(int damage)
    {
        // calculate damage with random range with max is int damage
        // damage = Random.Range(1, damage);
        // // print damage
        // print(unitName + " takes " + damage + " damage!");
        currentHP -= damage;
        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
