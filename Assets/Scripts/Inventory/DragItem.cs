using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    ItemUI curItemUI;   //当前拖拽对象
    SlotManager origin; 
    SlotManager target;
    private void Awake()
    {
        curItemUI = GetComponent<ItemUI>();
        origin = GetComponentInParent<SlotManager>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.curDrag = new InventoryManager.DragData();
        InventoryManager.Instance.curDrag.originalSlot = GetComponentInParent<SlotManager>();
        InventoryManager.Instance.curDrag.originalParent = (RectTransform)transform.parent;
        transform.SetParent(InventoryManager.Instance.DragCanvas.transform, true); //拖拽时位于dragcanvas上，从而位于UI最上层
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;  //实时跟随鼠标移动
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (InventoryManager.Instance.CheckinActionUI(eventData.position)|| InventoryManager.Instance.CheckinBagUI(eventData.position)
                || InventoryManager.Instance.CheckinStatusUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotManager>())
                {
                    target = eventData.pointerEnter.gameObject.GetComponent<SlotManager>();
                }
                else
                {
                    target = eventData.pointerEnter.gameObject.GetComponentInParent<SlotManager>();
                }
                switch (target.type)
                {
                    case SlotType.bag:
                        SwapItem();
                        break;
                    case SlotType.action:
                        if (curItemUI.bag.items[curItemUI.index].ItemData.type == ItemType.Consumable)
                            SwapItem();
                        break;
                    case SlotType.weapon:
                        if (curItemUI.bag.items[curItemUI.index].ItemData.type == ItemType.Weapon)
                            SwapItem();
                        break;
                    case SlotType.armor:
                        if (curItemUI.bag.items[curItemUI.index].ItemData.type == ItemType.Armor)
                            SwapItem();
                        break;
                }
                origin.UpdateItem();
                target.UpdateItem();
            }
        }
        //拖拽结束时回到原有父级元素（背包panel）
        transform.SetParent(InventoryManager.Instance.curDrag.originalParent);
        RectTransform t = transform as RectTransform;
        //保证格子拖拽结束后放在合适位置上
        t.offsetMax = -Vector2.one * 10;
        t.offsetMin = Vector2.one * 10;
    }
    public void SwapItem()
    {
        //target是slot，即一个格子，背包与格子是双向感知的，即一个背包知道我有哪些格子，每个格子也知道我属于哪个背包（也知道自己在这个背包中的序号），items是一个列表，用index取出对应序号的物品
        var targetItem = target.itemUI.bag.items[target.itemUI.index];
        var temp = origin.itemUI.bag.items[origin.itemUI.index];
        if (target.itemUI.bag == origin.itemUI.bag && target.itemUI.index == origin.itemUI.index)
        {
            return;  //同一格子
        }
        //Debug.Log(temp.ItemData.name);
        bool isSameItem = temp.ItemData == targetItem.ItemData;
        if (isSameItem && targetItem.ItemData.stackAmount > 1)  //可堆叠
        {
            targetItem.amount += temp.amount;
            temp.ItemData = null;
            temp.amount = 0;
        }
        else
        {
            //这里涉及到交换两个引用类型变量，origin,target,temp是3个地址值，起初origin和temp指向同一个对象，现在让origin和target分别指向对方原本所指的对象。
            //也就是说下面这一行并没有把origin指向的对象覆盖掉，只是把target这个地址变量的值赋给origin了而已，temp依然指向origin原本所指的对象。
            origin.itemUI.bag.items[origin.itemUI.index] = targetItem;
            //Debug.Log(temp.ItemData.name);
            target.itemUI.bag.items[target.itemUI.index] = temp;
        }
    }
}
