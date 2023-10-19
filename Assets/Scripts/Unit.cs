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
    public List<Skill> ReadySkills = new List<Skill>();
    public List<Skill> InitialSkills = new List<Skill>();
    public List<Skill> AlreadyUsedSkills = new List<Skill>();

    // unit side player or enemy
    public UnitSide unitSide;

    public DmgType weakness; // changed type to enum DmgType

    public UnitStatus.Status status;

    public void setInitialSkills()
    {
        foreach(Skill skill in skills)
        {
            InitialSkills.Add(skill);
        }
    }
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

public void SetupSkills()
{
    HashSet<Skill> uniqueSkills = new HashSet<Skill>(skills);

    // Jika ada kurang dari 5 skill unik, masukkan semuanya ke ReadySkills
    if (uniqueSkills.Count <= 5)
    {
        ReadySkills.AddRange(uniqueSkills);
    }
    else
    {
        // Jika ada lebih dari 5 skill unik, pilih 5 secara acak
        while (ReadySkills.Count < 5)
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
    // Hapus skill dari ReadySkills
    ReadySkills.Remove(usedSkill);
    Debug.Log("Removed skill from ReadySkills: " + usedSkill.Name);

    // Ambil skill acak baru dari InitialSkills
    Skill newSkill;
    do // Pastikan skill baru tidak ada di ReadySkills
    {
        int randIndex = Random.Range(0, InitialSkills.Count);
        newSkill = InitialSkills[randIndex];
    } while (ReadySkills.Contains(newSkill));

    ReadySkills.Add(newSkill); // Tambahkan skill baru ke ReadySkills
    Debug.Log("Added new skill to ReadySkills: " + newSkill.Name);
}
public void RefreshReadySkills()
{
    // Pastikan ReadySkills hanya memiliki 5 skill
    while (ReadySkills.Count > 5)
    {
        Skill skillToRemove = ReadySkills[0];
        ReadySkills.RemoveAt(0);
        InitialSkills.Remove(skillToRemove); // Hapus dari InitialSkills juga
    }
}
}
