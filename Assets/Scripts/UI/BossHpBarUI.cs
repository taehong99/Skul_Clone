using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBarUI : BaseUI
{
    [SerializeField] BossController boss;
    private void Start()
    {
        GetUI<Slider>("Slider").maxValue = boss.HP;
        GetUI<Slider>("Slider").value = boss.HP;
    }

    private void OnEnable()
    {
        SetHP(boss.HP);
        boss.OnHPChanged += SetHP;
    }

    private void OnDisable()
    {
        boss.OnHPChanged -= SetHP;
    }

    public void SetHP(int hp)
    {
        GetUI<Slider>("Slider").value = hp;
    }
}
