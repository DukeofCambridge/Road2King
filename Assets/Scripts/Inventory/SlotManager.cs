using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { bag,armor,weapon,action} //action bar指可以随时使用的随身物品（类似Terraria快捷键1-9），bag是背包
public class SlotManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemUI itemUI;
    public SlotType type;
    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)//双击
        {
            UseItem();
        }
    }
    public void UseItem()
    {
        if(itemUI.GetItem()!=null&& itemUI.GetItem().type == ItemType.Consumable)
        {
            itemUI.bag.items[itemUI.index].amount-=1;
            GameManager.Instance.playerData.Heal(itemUI.GetItem().consumeData.healPoint);

            if (itemUI.GetAmount() == 0)
            {
                itemUI.bag.items[itemUI.index].ItemData = null;
            }
        }
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (type)
        {
            case SlotType.bag:
                itemUI.bag = InventoryManager.Instance.BagData;
                break;
            case SlotType.armor:
                itemUI.bag = InventoryManager.Instance.StatusData;
                break;
            case SlotType.weapon:
                itemUI.bag = InventoryManager.Instance.StatusData;
                if (itemUI.GetItem() != null)
                {
                    GameManager.Instance.playerData.ChangeWeapon(itemUI.GetItem());
                }
                else
                {
                    GameManager.Instance.playerData.Unwield();
                }
                break;
            case SlotType.action:
                itemUI.bag = InventoryManager.Instance.ActionData;
                break;
        }
        var item = itemUI.bag.items[itemUI.index]; //找到对应背包的对应位置的数据
        //Debug.Log(item.ItemData.name);
        itemUI.UpdateUI(item.ItemData, item.amount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetUpTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
            InventoryManager.Instance.tooltip.UpdatePosition();
        }
    }
}
