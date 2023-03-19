using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    ItemUI curItemUI;   //��ǰ��ק����
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
        transform.SetParent(InventoryManager.Instance.DragCanvas.transform, true); //��קʱλ��dragcanvas�ϣ��Ӷ�λ��UI���ϲ�
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;  //ʵʱ��������ƶ�
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
        //��ק����ʱ�ص�ԭ�и���Ԫ�أ�����panel��
        transform.SetParent(InventoryManager.Instance.curDrag.originalParent);
        RectTransform t = transform as RectTransform;
        //��֤������ק��������ں���λ����
        t.offsetMax = -Vector2.one * 10;
        t.offsetMin = Vector2.one * 10;
    }
    public void SwapItem()
    {
        //target��slot����һ�����ӣ������������˫���֪�ģ���һ������֪��������Щ���ӣ�ÿ������Ҳ֪���������ĸ�������Ҳ֪���Լ�����������е���ţ���items��һ���б���indexȡ����Ӧ��ŵ���Ʒ
        var targetItem = target.itemUI.bag.items[target.itemUI.index];
        var temp = origin.itemUI.bag.items[origin.itemUI.index];
        if (target.itemUI.bag == origin.itemUI.bag && target.itemUI.index == origin.itemUI.index)
        {
            return;  //ͬһ����
        }
        //Debug.Log(temp.ItemData.name);
        bool isSameItem = temp.ItemData == targetItem.ItemData;
        if (isSameItem && targetItem.ItemData.stackAmount > 1)  //�ɶѵ�
        {
            targetItem.amount += temp.amount;
            temp.ItemData = null;
            temp.amount = 0;
        }
        else
        {
            //�����漰�����������������ͱ�����origin,target,temp��3����ֵַ�����origin��tempָ��ͬһ������������origin��target�ֱ�ָ��Է�ԭ����ָ�Ķ���
            //Ҳ����˵������һ�в�û�а�originָ��Ķ��󸲸ǵ���ֻ�ǰ�target�����ַ������ֵ����origin�˶��ѣ�temp��Ȼָ��originԭ����ָ�Ķ���
            origin.itemUI.bag.items[origin.itemUI.index] = targetItem;
            //Debug.Log(temp.ItemData.name);
            target.itemUI.bag.items[target.itemUI.index] = temp;
        }
    }
}
