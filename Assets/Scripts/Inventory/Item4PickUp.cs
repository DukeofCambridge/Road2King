using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item4PickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    //boxColliderû�д�������Ϊ������������ײ��capsuleCollider�д�������Ϊ��������ܼ�����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.BagData.AddItem(itemData, itemData.amount);
            InventoryManager.Instance.BagUI.UpdateUI();
            //GameManager.Instance.playerData.SetUp(itemData);
            Destroy(gameObject);
        }
    }
}
