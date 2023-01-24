using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;

    private void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    private void Update()
    {
        levelText.text = "Level  " + GameManager.Instance.playerData.characterData.curLevel.ToString("00");
        UpdateHealth();
        UpdateEXP();
    }
    void UpdateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerData.curHealth / GameManager.Instance.playerData.characterData.maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    void UpdateEXP()
    {
        float sliderPercent = (float)GameManager.Instance.playerData.characterData.exp / GameManager.Instance.playerData.characterData.exp4levelUP;
        expSlider.fillAmount = sliderPercent;
    }
}
