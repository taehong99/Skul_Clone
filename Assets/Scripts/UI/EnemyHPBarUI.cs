using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarUI : BaseUI
{
    [SerializeField] Enemy enemy;
    RectTransform rect;

    private void Start()
    {
        GetUI<Slider>("Slider").maxValue = enemy.HP;
        GetUI<Slider>("Slider").value = enemy.HP;
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        SetHP(enemy.HP);
        enemy.OnHPChanged += SetHP;
        enemy.OnFlipped += FlipScale;
    }

    private void OnDisable()
    {
        enemy.OnHPChanged -= SetHP;
        enemy.OnFlipped -= FlipScale;
    }

    public void SetHP(int hp)
    {
        GetUI<Slider>("Slider").value = hp;
    }

    public void FlipScale()
    {
        Vector3 newScale = rect.localScale;
        newScale.x = newScale.x * -1;
        rect.localScale = newScale;
    }
}
