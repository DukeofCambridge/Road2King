using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    public bool broken;       //动作被打断

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        //以该类型怪物的数据作为模板，分别做一份拷贝给该类型的每个怪物
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
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
    public void takeDamage(CharacterData attacker,CharacterData defender)
    {
        //结算伤害=攻击者修正伤害-受击者防御力
        int damage = Mathf.Max(attacker.revisedDamage() - defender.totalDefence,0);
        curHealth = Mathf.Max(curHealth - damage, 0);
        //暴击时触发被攻击者的受击动画
        if (attacker.isCritical)
        {
            //Debug.Log("暴击攻击！");
            defender.broken = true;  //打断攻击
            defender.GetComponent<Animator>().SetTrigger("hit");
            
        }
    }
    public void takeDamage(int damage, CharacterData defender)
    {
        int calcDamage = Mathf.Max(damage - defender.totalDefence, 0);
        curHealth = Mathf.Max(curHealth - calcDamage, 0);
        defender.broken = true;  //打断攻击
        defender.GetComponent<Animator>().SetTrigger("hit");
        
    }
    //攻击者的修正伤害
    private int revisedDamage()
    {
        float damage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            damage *= attackData.criticalMultiplier;
        }
        return (int)damage;
    }

    #endregion
}
