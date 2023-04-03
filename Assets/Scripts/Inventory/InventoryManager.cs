using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData{
        public SlotManager originalSlot;
        public RectTransform originalParent;
    }
    [Header("UI Panel")]
    public GameObject BagPanel;
    public GameObject StatusPanel;
    bool isOpen = false;
    [Header("Player Data")]
    public Text maxHealthText;
    public Text atkText;
    [Header("Inventory Data")] 
    public InventoryData_SO TemplateBagData;
    public InventoryData_SO TemplateActionData;
    public InventoryData_SO TemplateStatusData;
    public InventoryData_SO BagData;     //背包
    public InventoryData_SO ActionData;  //行动栏
    public InventoryData_SO StatusData;  //角色当前装备
    [Header("Inventory UI")]
    public InventoryUI BagUI;
    public InventoryUI ActionUI;
    public InventoryUI StatusUI;
    [Header("Drag Canvas")]
    public Canvas DragCanvas;
    public DragData curDrag;
    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        if (TemplateBagData != null)
            BagData = Instantiate(TemplateBagData);
        if (TemplateActionData != null)
            ActionData = Instantiate(TemplateActionData);
        if (TemplateStatusData != null)
            StatusData = Instantiate(TemplateStatusData);
    }

    private void Start()
    {
        Instance.BagUI.UpdateUI();
        Instance.ActionUI.UpdateUI();
        Instance.StatusUI.UpdateUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            BagPanel.SetActive(isOpen);
            StatusPanel.SetActive(isOpen);
        }
    }
    //升级或吃药或更换装备时调用
    public void UpdatePlayerDataText(int health,float min,float max)
    {
        maxHealthText.text = health.ToString();
        atkText.text = min.ToString() + " ~ " + max.ToString();
    }
    public void UpdatePlayerDataText(int health)
    {
        maxHealthText.text = health.ToString();
    }
    #region 检查拖拽物品在哪个slot内
    public bool CheckinBagUI(Vector3 p)
    {
        for(int i = 0; i < BagUI.slots.Length; ++i)
        {
            RectTransform rt = BagUI.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, p))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckinActionUI(Vector3 p)
    {
        for (int i = 0; i < ActionUI.slots.Length; ++i)
        {
            RectTransform rt = ActionUI.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, p))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckinStatusUI(Vector3 p)
    {
        for (int i = 0; i < StatusUI.slots.Length; ++i)
        {
            RectTransform rt = StatusUI.slots[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, p))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    public void SaveInventory()
    {
        SaveManager.Instance.Save(BagData,BagData.name);
        SaveManager.Instance.Save(ActionData,ActionData.name);
        SaveManager.Instance.Save(StatusData,StatusData.name);
    }
    public void LoadInventory()
    {
        SaveManager.Instance.Load(BagData,BagData.name);
        SaveManager.Instance.Load(ActionData,ActionData.name);
        SaveManager.Instance.Load(StatusData,StatusData.name);
    }
}
