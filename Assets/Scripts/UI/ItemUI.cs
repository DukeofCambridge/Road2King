using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image Icon = null;
    public Text Amount = null;
    public InventoryData_SO bag { get; set; }
    public int index { get; set; } = -1;

    public void UpdateUI(ItemData_SO data,int amount)
    {
        if (data != null&&amount!=0)
        {
            Icon.sprite = data.icon;
            Amount.text = amount.ToString();
            Icon.gameObject.SetActive(true);
        }
        else
        {
            Icon.gameObject.SetActive(false);
        }
    }
    public ItemData_SO GetItem()
    {
        return bag.items[index].ItemData;
    }
    public int GetAmount()
    {
        return bag.items[index].amount;
    }
}
