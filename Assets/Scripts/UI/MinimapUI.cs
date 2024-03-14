using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class MinimapUI : BaseUI
{
    TextMeshProUGUI enemyCountText;
    int count = 0;

    private void Start()
    {
        enemyCountText = GetUI<TextMeshProUGUI>("EnemyCount");
        enemyCountText.text = count.ToString();
        Manager.Events.voidEventDic["enemySpawned"].OnEventRaised += IncreaseCount;
        Manager.Events.voidEventDic["enemyKilled"].OnEventRaised += DecreaseCount;
    }

    public void IncreaseCount()
    {
        count++;
        enemyCountText.text = count.ToString();
    }

    public void DecreaseCount()
    {
        count--;
        enemyCountText.text = count.ToString();
    }
}
