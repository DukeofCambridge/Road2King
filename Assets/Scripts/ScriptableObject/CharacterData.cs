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
    public bool broken;       //���������
    [Header("Weapon")]
    public Transform weaponPos;
    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        //�Ը����͹����������Ϊģ�壬�ֱ���һ�ݿ����������͵�ÿ������
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
    //ֱ���˺�
    public void TakeDamage(CharacterData attacker,CharacterData defender)
    {
        //�����˺�=�����������˺�-�ܻ��߷�����
        int damage = Mathf.Max(attacker.RevisedDamage() - defender.totalDefence,0);
        curHealth = Mathf.Max(curHealth - damage, 0);
        //����ʱ�����������ߵ��ܻ�����
        if (attacker.isCritical)
        {
            //Debug.Log("����������");
            defender.broken = true;  //��Ϲ���
            defender.GetComponent<Animator>().SetTrigger("hit");
            
        }
        //����UI
        UpdateHealthBarOnAttacked?.Invoke(curHealth, maxHealth);
        if (curHealth <= 0)
        {
            //��ɱ����Ϊ����ṩEXP
            attacker.characterData.UpdateExp(characterData.rewardEXP);
            //TODO:����������������
        }
    }
    //��ֱ���˺�����ʯͷײ����
    public void TakeDamage(int damage, CharacterData defender)
    {
        int calcDamage = Mathf.Max(damage - defender.totalDefence, 0);
        curHealth = Mathf.Max(curHealth - calcDamage, 0);
        defender.broken = true;  //��Ϲ���
        defender.GetComponent<Animator>().SetTrigger("hit");
        //����UI
        UpdateHealthBarOnAttacked?.Invoke(curHealth, maxHealth);
        if (curHealth <= 0)
        {
            GameManager.Instance.playerData.characterData.UpdateExp(characterData.rewardEXP);
        }
    }
    //�����ߵ������˺�
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
    #region װ������
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
        attackData.SetUp(bas_atkData); //��ԭĬ��ֵ
        InventoryManager.Instance.UpdatePlayerDataText(maxHealth, attackData.minDamage, attackData.maxDamage);
    }
    #endregion
    public void Heal(int point)
    {
        curHealth = curHealth + point > maxHealth ? maxHealth : curHealth + point;
    }
}
