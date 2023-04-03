using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour,IDragHandler,IPointerDownHandler
{
    private RectTransform _rt;
    private Canvas _canvas;
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvas = InventoryManager.Instance.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rt.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //保证当前被拖拽高于其他背包，但低于DragCanvas和ToolTip
        _rt.SetSiblingIndex(2);
    }
}
