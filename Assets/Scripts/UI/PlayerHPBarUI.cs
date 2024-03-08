using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPBarUI : BaseUI
{
    [SerializeField] PlayerController player;
    private int maxHP;

    private void Start()
    {
        maxHP = player.HP;
        GetUI<Slider>("HealthUI").maxValue = maxHP;
        GetUI<Slider>("HealthUI").value = maxHP;
        GetUI<TextMeshProUGUI>("HealthText").text = $"{maxHP}/{maxHP}";
    }

    private void OnEnable()
    {
        SetHP(player.HP);
        player.OnHPChanged += SetHP;
    }

    private void OnDisable()
    {
        player.OnHPChanged -= SetHP;
    }

    public void SetHP(int hp)
    {
        GetUI<Slider>("HealthUI").value = hp;
        GetUI<TextMeshProUGUI>("HealthText").text = $"{hp}/{maxHP}";
    }
}
