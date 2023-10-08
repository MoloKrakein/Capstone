using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DmgType", menuName = "MyGame/DmgType", order = 0)]
public class DmgType : ScriptableObject
{
    public enum Type
    {
        Physical,
        Fire,
        Earth,
        Darkness,
        Light,
    }

    [System.Serializable]
    public class DamageInfo
    {
        public int ManaCost;
        public bool UsesHP;
    }

    public List<DamageInfo> damageInfos = new List<DamageInfo>();

    public void Initialize()
    {
        damageInfos.Add(new DamageInfo { ManaCost = 0, UsesHP = true });     // Physical
        damageInfos.Add(new DamageInfo { ManaCost = 15, UsesHP = false });   // Fire
        damageInfos.Add(new DamageInfo { ManaCost = 12, UsesHP = false });   // Earth
        damageInfos.Add(new DamageInfo { ManaCost = 20, UsesHP = false });   // Darkness
        damageInfos.Add(new DamageInfo { ManaCost = 25, UsesHP = false });   // Light
    }
}
