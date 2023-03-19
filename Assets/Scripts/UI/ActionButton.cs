using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;
    private SlotManager slot;
    private void Awake()
    {
        slot = GetComponent<SlotManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(actionKey) && slot.itemUI.GetItem() != null)
        {
            slot.UseItem();
        }
    }
}
