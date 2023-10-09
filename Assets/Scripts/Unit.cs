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

    public int maxMP;
    public int currentMP;
    public DmgType.Type weakness;

    public UnitStatus.Status status;

    public bool TakeDamage(int damage, DmgType.Type AttackType)
    {   
        if(UnitStatus.Status.Down == status)
        {
            damage *= 2;
            status = UnitStatus.Status.Down;
        }

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

    public bool isWeakness(DmgType.Type AttackType)
    {
        if(weakness == AttackType)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
