using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable/Skill", order = 0)]
public class Skill : ScriptableObject
{
    public string Name;
    public int AttackPower;
    public DmgType AttackType;
    public bool targetsAllEnemies;
    public int ManaCost;
    public bool UsesHP;
    public string info;
}