using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : BaseUI
{
    [SerializeField] int skillSlot;
    PlayerController player;

    //private void Awake()
    //{
    //    player = Manager.Game.Player;
    //    GetUI<Image>("Skill1Cooldown").fillAmount = 0;
    //}

    private void Start()
    {
        player = Manager.Game.Player;
    }

    private void Update()
    {
        //TODO : Refactor player skill cooldown tracking
        //GetUI<Image>("Skill1Cooldown").fillAmount = player.CooldownRatio;
        //GetUI<Image>("Skill2Cooldown").enabled = !player.CanTeleport;
    }

    private void UpdateUI()
    {

    }
}
