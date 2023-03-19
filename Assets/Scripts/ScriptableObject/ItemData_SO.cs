using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Weapon,Armor,Consumable}
[CreateAssetMenu(fileName ="New Data",menuName ="Scriptable Object/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType type;
    public string name;
    public Sprite icon;
    public int amount;
    [TextArea]
    public string description = "";
    public int stackAmount;  //可堆叠数上限（一个格子最多放多少个）
    [Header("Weapon")]
    public GameObject prefab;
    public AttackData_SO weaponData;
    [Header("Consumables")]
    public ConsumableData_SO consumeData;
}
