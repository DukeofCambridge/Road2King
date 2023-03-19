using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Data", menuName = "Scriptable Object/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float cooldown;
    public float criticalMultiplier;
    public float criticalRate;
    public float minDamage;
    public float maxDamage;
    public void SetUp(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        cooldown = weapon.cooldown;
        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;
    }
}
