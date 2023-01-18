using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;
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
        //�����˺�=�����������˺�-�ܻ��߷�����
        int damage = Mathf.Max(attacker.revisedDamage() - defender.totalDefence,0);
        curHealth = Mathf.Max(curHealth - damage, 0);
        //����ʱ�����������ߵ��ܻ�����
        if (isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("hit");
        }
    }
    //�����ߵ������˺�
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
