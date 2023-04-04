using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text nameText;
    public Text infoText;
    RectTransform rt;
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        UpdatePosition();
    }
    private void Update()
    {
        UpdatePosition();
    }
    public void SetUpTooltip(ItemData_SO item)
    {
        nameText.text = item.name;
        infoText.text = item.description;
    }
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float width = corners[3].x - corners[0].x;
        float height = corners[1].y - corners[0].y;
        //根据鼠标在屏幕的位置决定ToolTip生成的位置
        if (mousePos.y < height)
        {
            rt.position = mousePos + Vector3.up * height*0.6f;
        }else if (Screen.width - mousePos.x > width)
        {
            rt.position = mousePos + Vector3.right * width * 0.6f;
        }
        else
        {
            rt.position = mousePos + Vector3.left * width * 0.6f;
        }

    }
}
