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
        for (int i = 0; i < 5; i++) // Ambil 5 skill acak dari list skills
        {
            int randIndex = Random.Range(0, skills.Count);
            ReadySkills.Add(skills[randIndex]);
        }
    }

    public void HandleUsedSkill(Skill usedSkill)
    {
        // Tambahkan skill ke AlreadyUsedSkills
        usedSkill.turnsSinceUsed = 0; // Reset turnsSinceUsed
        AlreadyUsedSkills.Add(usedSkill);

        // Hapus skill dari ReadySkills
        ReadySkills.Remove(usedSkill);

        // Ambil skill acak baru dari InitialSkills
        int randIndex;
        Skill newSkill;
        do // Pastikan skill baru tidak sama dengan skill sebelumnya
        {
            randIndex = Random.Range(0, InitialSkills.Count);
            newSkill = InitialSkills[randIndex];
        } while (newSkill == usedSkill);

        ReadySkills.Add(newSkill); // Tambahkan skill baru ke ReadySkills
    }

     public void RefreshReadySkills()
    {
        // Pindahkan skill dari AlreadyUsedSkills ke ReadySkills jika sudah 3 turns
        for (int i = 0; i < AlreadyUsedSkills.Count; i++)
        {
            Skill skill = AlreadyUsedSkills[i];
            skill.turnsSinceUsed++;

            if (skill.turnsSinceUsed >= 3)
            {
                AlreadyUsedSkills.RemoveAt(i);
                i--; // Mengurangi i karena kita menghapus item dari list
                ReadySkills.Add(skill);
            }
        }
    }
}
