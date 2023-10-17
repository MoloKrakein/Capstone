using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitSide { Player, Enemy };
public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;

    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;

    public int speed;
    public bool isDown = false;
    public bool hasExtraTurn;

    // Use a list of skills instead of a skill set
    public List<Skill> skills = new List<Skill>();

    // unit side player or enemy
    public UnitSide unitSide;

    public DmgType weakness; // changed type to enum DmgType

    public UnitStatus.Status status;

    public void TakeDamage(int damage, DmgType attackType)
    {
        if(UnitStatus.Status.Down == status)
        {
            damage *= 2;
            status = UnitStatus.Status.Down;
        }

        currentHP -= damage;
        Debug.Log(unitName + " took " + damage + " damage!");
    }

    public bool isDead()
    {
        if(currentHP <= 0)
        {
            status = UnitStatus.Status.Dead;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool isWeakness(DmgType attackType)
    {
        if(weakness == attackType)
        {
            status = UnitStatus.Status.Down;
            isDown = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
