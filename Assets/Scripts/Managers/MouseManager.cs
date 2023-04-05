using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//通过拖拽方式+引擎内可视化面板 来操作点击事件
//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{
    //鼠标样式
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;
    //public EventVector3 OnMouseClicked;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        setCursorTexture();
        //与UI交互则不触发大世界点击事件
        if (isInteractWithUI()) return;
        mouseControl();
    }
    void mouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Item"))
                OnMouseClicked?.Invoke(hitInfo.point);
        }
    }
    void setCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //与UI交互锁定鼠标样式
        if (isInteractWithUI())
        {
            Cursor.SetCursor(point, Vector2.zero, CursorMode.Auto);
            return;
        }
        if(Physics.Raycast(ray,out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
    bool isInteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
