using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarUI : BaseUI
{
    [SerializeField] Enemy enemy;

    private void Start()
    {
        GetUI<Slider>("Slider").maxValue = enemy.HP;
        GetUI<Slider>("Slider").value = enemy.HP;
    }

    private void OnEnable()
    {
        SetHP(enemy.HP);
        enemy.OnHPChanged += SetHP;
    }

    private void OnDisable()
    {
        enemy.OnHPChanged -= SetHP;
    }

    public void SetHP(int hp)
    {
        GetUI<Slider>("Slider").value = hp;
    }
}
