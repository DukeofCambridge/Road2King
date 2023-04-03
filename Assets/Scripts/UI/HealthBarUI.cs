using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject HealthBarPrefab;
    public Transform barPos;
    public bool AlwaysVisible;
    Image healthSlider;
    Transform UIbar;
    Transform camera;
    CharacterData curData;
    public float visibleTime;
    private float visibleTimeLeft;
    private void Awake()
    {
        curData = GetComponent<CharacterData>();
        curData.UpdateHealthBarOnAttacked += UpdateHealthBar;
    }
    private void OnEnable()
    {
        camera = Camera.main.transform;
        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIbar = Instantiate(HealthBarPrefab, canvas.transform).transform;
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(AlwaysVisible);
            }
        }
    }
    //UI渲染和更新应该放在LateUpdate部分，防止镜头闪烁
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPos.position;
            //保持面向镜头
            UIbar.forward = -camera.forward;
            if (visibleTimeLeft <= 0 && !AlwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else visibleTimeLeft -= Time.deltaTime;
        }
    }
    private void UpdateHealthBar(int curHealth,int maxHealth)
    {
        if (curHealth <= 0)
        {
            Destroy(UIbar.gameObject);
        }
        UIbar.gameObject.SetActive(true);
        visibleTimeLeft = visibleTime;
        //根据血量百分比设置UI
        float sliderPercent = (float)curHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
}
