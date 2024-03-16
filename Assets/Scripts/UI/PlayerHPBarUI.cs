using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHPBarUI : BaseUI
{
    GameManager game;
    private int maxHP;

    private void Start()
    {
        game = Manager.Game;
        maxHP = game.PlayerHP;
        SetHP(maxHP);
        game.OnPlayerHPChanged += SetHP;

        GetUI<Slider>("HealthUI").maxValue = maxHP;
        GetUI<Slider>("HealthUI").value = maxHP;
        GetUI<TextMeshProUGUI>("HealthText").text = $"{maxHP}/{maxHP}";
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        game.OnPlayerHPChanged -= SetHP;
    }

    public void SetHP(int hp)
    {
        GetUI<Slider>("HealthUI").value = hp;
        GetUI<TextMeshProUGUI>("HealthText").text = $"{hp}/{maxHP}";
    }
}
