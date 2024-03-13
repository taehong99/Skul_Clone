using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBackground : BaseUI
{
    [SerializeField] Sprite phase2Background;
    [SerializeField] Sprite neutralBackground;
    

    private void Start()
    {
        Manager.Events.voidEventDic["phase2Started"].OnEventRaised += ChangeToPhase2Background;
        Manager.Events.voidEventDic["bossDefeated"].OnEventRaised += ChangeToNeutralBackground;
    }

    private void ChangeToPhase2Background()
    {
        GetUI<Image>("Background").sprite = phase2Background;
    }

    private void ChangeToNeutralBackground()
    {
        GetUI<Image>("Background").sprite = neutralBackground;
    }
}
