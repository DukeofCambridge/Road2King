using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public SlotManager[] slots;
    public void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].itemUI.index = i;
            slots[i].UpdateItem();
        }
    }
}
