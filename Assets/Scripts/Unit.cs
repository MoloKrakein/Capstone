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

    // public int speed;
    // public bool isDown = false;
    // public bool hasExtraTurn;

    // Use a list of skills instead of a skill set
    public List<Skill> skills = new List<Skill>();
    public List<Skill> ReadySkills = new List<Skill>();
    // public List<Skill> InitialSkills = new List<Skill>();
    public List<Skill> AlreadyUsedSkills = new List<Skill>();


    // unit side player or enemy
    public UnitSide unitSide;

    public DmgType weakness; // changed type to enum DmgType

    public UnitStatus.Status status;


    public void TakeDamage(int damage, DmgType attackType)
    {
        if(UnitStatus.Status.Down == status)
        {
            damage *= 2;
        }

        currentHP -= damage;
        Debug.Log(unitName + " took " + damage + " ");
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

    public bool isDown(){
        if(UnitStatus.Status.Down == status){
            return true;
        }else{
            return false;
        }
    }
    
    public bool isWeakness(DmgType attackType)
    {
        if(weakness == attackType)
        {
            status = UnitStatus.Status.Down;
            return true;
        }
        else
        {
            return false;
        }
    }

public void SetupSkills()
    {
        HashSet<Skill> uniqueSkills = new HashSet<Skill>(skills);

        // Jika ada kurang dari 5 skill unik, masukkan semuanya ke ReadySkills
        if (uniqueSkills.Count <= 6)
        {
            ReadySkills.Clear();
            ReadySkills.AddRange(uniqueSkills);
        }
        else
        {
            // Jika ada lebih dari 5 skill unik, pilih 5 secara acak
            ReadySkills.Clear();
            while (ReadySkills.Count < 6)
            {
                int randIndex = Random.Range(0, skills.Count);
                Skill skill = skills[randIndex];

                if (!ReadySkills.Contains(skill)) // Memastikan skill unik
                {
                    ReadySkills.Add(skill);
                }
            }
        }
    }
public void HandleUsedSkill(Skill usedSkill)
    {
        AlreadyUsedSkills.Add(usedSkill);
        int index = skills.IndexOf(usedSkill);
        // print log skill index
        Debug.Log("skill index : " + index);
        Skill newSkill;
        // isi newSkill yang tidak ada di AlreadyUsedSkills dan juga di ReadySkills dari skills
        do
        {
            newSkill = skills[Random.Range(0, skills.Count)];
        } while (AlreadyUsedSkills.Contains(newSkill));
     
        SwapSkill(usedSkill, newSkill);

        if (AlreadyUsedSkills.Count >= skills.Count/2)
        {
            AlreadyUsedSkills.RemoveAt(0);
        }
    }


// clear already used skill
public void ClearAlreadyUsedSkills()
{
    AlreadyUsedSkills.Clear();
}

private void SwapSkill(Skill skill1, Skill skill2)
{
    int index1 = ReadySkills.IndexOf(skill1);
    int index2 = ReadySkills.IndexOf(skill2);

    ReadySkills[index1] = skill2;
    ReadySkills[index2] = skill1;

}
}
