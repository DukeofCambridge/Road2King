using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName = "Scriptable Object/Character Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Basic Stats")]
    public int maxHealth;
    public int curHealth;
    public int basDefence;
    public int totalDefence;
    [Header("Kill Reward")]
    public int rewardEXP;
    [Header("Level Stats")]
    public int curLevel;
    public int maxLevel;
    public int exp;            //�ۼƾ���ֵ
    public int exp4levelUP;    //������һ�������ۼƾ���ֵ
    public float levelBuff;    //���ڸ��ݵȼ���̬����������������
    public float levelMultiplier //����ֵ��������(����)
    {
        get { return 1 + (curLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        exp += point;
        if (exp >= exp4levelUP)
        {
            levelUP();
        }
    }
    private void levelUP()
    {
        curLevel = Mathf.Clamp(curLevel + 1, 0,maxLevel);
        exp4levelUP += (int)(exp4levelUP * levelMultiplier);

        maxHealth = (int)(maxHealth * (1+levelBuff));
        curHealth = maxHealth;
    }
}
