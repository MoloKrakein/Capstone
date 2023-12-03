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
        // Hapus skill dari ReadySkills
        ReadySkills.Remove(usedSkill);

        if (skills.Count > 0)
        {
            Skill newSkill;
            do // Pastikan skill baru tidak ada di ReadySkills
            {
                int randIndex = Random.Range(0, skills.Count);
                newSkill = skills[randIndex];
            } while (ReadySkills.Contains(newSkill));

            ReadySkills.Add(newSkill);
        }
        // CheckSkills(ReadySkill, UsedSkill);
        // Jika AlreadyUsedSkills sudah mencapai batas, hapus yang paling awal
        if (AlreadyUsedSkills.Count >= skills.Count/2)
        {
            AlreadyUsedSkills.RemoveAt(0);
        }
    }
public void RefreshReadySkills()
{
    // Pastikan ReadySkills hanya memiliki 5 skill
    while (ReadySkills.Count > 6)
    {
        ReadySkills.RemoveAt(4);
    }
    // ReadySkills.AddRange(InitialSkills); // Tambahkan skill dari InitialSkills ke ReadySkills
}

// clear already used skill
public void ClearAlreadyUsedSkills()
{
    AlreadyUsedSkills.Clear();
}

// private void CheckSkills(Skill ReadySkill,Skill UsedSkill){
//     if(ReadySkill == UsedSkill){
//         // get readyskill index
//         int index = ReadySkills.IndexOf(ReadySkill);
//         ReadySkills.RemoveAt(index);
//         // add random skill
//         Skill newSkill;
//         do // Pastikan skill baru tidak ada di ReadySkills
//         {
//             int randIndex = Random.Range(0, skills.Count);
//             newSkill = skills[randIndex];
//         } while (ReadySkills.Contains(newSkill));

//     }
// }

}

