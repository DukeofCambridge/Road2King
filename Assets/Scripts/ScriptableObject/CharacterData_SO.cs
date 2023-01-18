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
}
