using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item4PickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    //boxCollider没有触发器是为了与地面进行碰撞，capsuleCollider有触发器是为了让玩家能捡起来
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
