using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Scriptable Object/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public void AddItem(ItemData_SO itemData,int amount)
    {
        int found = 0;
        if (itemData.stackAmount > 1)  //可堆叠物品
        {
            for(int i=0;i<items.Count;i++)
            {
                if (items[i].ItemData!=null&&items[i].ItemData.name == itemData.name)
                {
                    //判断是否达到堆叠上限
                    int remain = items[i].ItemData.stackAmount - items[i].amount;
                    if (remain >= amount)
                    {
                        items[i].amount += amount;
                        return;
                    }
                    else
                    {
                        items[i].amount = items[i].ItemData.stackAmount;
                        amount -= remain;
                        found = i;
                    }
                }
            }
        }

        for(int i = found; i < items.Count; i++)
        {
            if (items[i].ItemData == null)
            {
                items[i].ItemData = itemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}
[System.Serializable]
public class InventoryItem
{
    public ItemData_SO ItemData;
    public int amount;
}