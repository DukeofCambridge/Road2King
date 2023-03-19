using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttacked;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    public AttackData_SO bas_atkData;
    public bool broken;       //动作被打断
    [Header("Weapon")]
    public Transform weaponPos;
    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        //以该类型怪物的数据作为模板，分别做一份拷贝给该类型的每个怪物
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        bas_atkData = Instantiate(attackData);
        InventoryManager.Instance.UpdatePlayerDataText(maxHealth, attackData.minDamage, attackData.maxDamage);
    }
    #region R/W from Data_SO
    public int maxHealth
    {
        get { return characterData != null? characterData.maxHealth: 0; }
        set { characterData.maxHealth = value; }
    }
    public int curHealth
    {
        get { return characterData != null ? characterData.curHealth : 0; }
        set { characterData.curHealth = value; }
    }
    public int basDefence
    {
        get { return characterData != null ? characterData.basDefence : 0; }
        set { characterData.basDefence = value; }
    }
    public int totalDefence
    {
        get { return characterData != null ? characterData.totalDefence : 0; }
        set { characterData.totalDefence = value; }
    }
    #endregion
    #region Battle_Calc
    //直接伤害
    public void TakeDamage(CharacterData attacker,CharacterData defender)
    {
        //结算伤害=攻击者修正伤害-受击者防御力
        int damage = Mathf.Max(attacker.RevisedDamage() - defender.totalDefence,0);
        curHealth = Mathf.Max(curHealth - damage, 0);
        //暴击时触发被攻击者的受击动画
        if (attacker.isCritical)
        {
            //Debug.Log("暴击攻击！");
            defender.broken = true;  //打断攻击
            defender.GetComponent<Animator>().SetTrigger("hit");
            
        }
        //更新UI
        UpdateHealthBarOnAttacked?.Invoke(curHealth, maxHealth);
        if (curHealth <= 0)
        {
            //击杀敌人为玩家提供EXP
            attacker.characterData.UpdateExp(characterData.rewardEXP);
            //TODO:提升攻击力防御力
        }
    }
    //非直接伤害（如石头撞击）
    public void TakeDamage(int damage, CharacterData defender)
    {
        int calcDamage = Mathf.Max(damage - defender.totalDefence, 0);
        curHealth = Mathf.Max(curHealth - calcDamage, 0);
        defender.broken = true;  //打断攻击
        defender.GetComponent<Animator>().SetTrigger("hit");
        //更新UI
        UpdateHealthBarOnAttacked?.Invoke(curHealth, maxHealth);
        if (curHealth <= 0)
        {
            GameManager.Instance.playerData.characterData.UpdateExp(characterData.rewardEXP);
        }
    }
    //攻击者的修正伤害
    private int RevisedDamage()
    {
        float damage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            damage *= attackData.criticalMultiplier;
        }
        return (int)damage;
    }

    #endregion
    #region 装备穿脱
    public void ChangeWeapon(ItemData_SO weapon)
    {
        Unwield();
        EquipWeapon(weapon);
    }
    public void EquipWeapon(ItemData_SO weapon)
    {
        if (weapon.prefab != null)
        {
            Instantiate(weapon.prefab, weaponPos);
        }
        attackData.SetUp(weapon.weaponData);
        InventoryManager.Instance.UpdatePlayerDataText(maxHealth, attackData.minDamage, attackData.maxDamage);
    }
    public void Unwield()
    {
        if (weaponPos.transform.childCount != 0)
        {
            for(int i = 0; i < weaponPos.transform.childCount;++i)
            {
                Destroy(weaponPos.transform.GetChild(i).gameObject);
            }
        }
        attackData.SetUp(bas_atkData); //还原默认值
        InventoryManager.Instance.UpdatePlayerDataText(maxHealth, attackData.minDamage, attackData.maxDamage);
    }
    #endregion
    public void Heal(int point)
    {
        curHealth = curHealth + point > maxHealth ? maxHealth : curHealth + point;
    }
}
