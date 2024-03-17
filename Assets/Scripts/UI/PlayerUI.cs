using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : BaseUI
{
    PlayerController player;

    //private void Awake()
    //{
    //    player = Manager.Game.Player;
    //    GetUI<Image>("Skill1Cooldown").fillAmount = 0;
    //}

    private void Start()
    {
        Debug.Log("PLAYER UI START");
        player = Manager.Game.Player;
        Manager.Events.dataEventDic["skullPickedUp"].OnEventRaised += PickUpSkull;
        Manager.Events.voidEventDic["skullSwapped"].OnEventRaised += UpdateIcons;
    }

    private void Update()
    {
        GetUI<Image>("Skill1Cooldown").fillAmount = player.Skill1CooldownRatio;
        GetUI<Image>("Skill2Cooldown").fillAmount = player.Skill2CooldownRatio;
        GetUI<Image>("SwapCooldown").fillAmount = player.SwapCooldownRatio;
    }

    private void PickUpSkull(PlayerData data)
    {
        GetUI<Image>("MainSkullIcon").sprite = data.mainIcon;
    }

    private void UpdateIcons()
    {
        player = Manager.Game.Player;
        GetUI<Image>("MainSkullIcon").sprite = player.Data.mainIcon;
        GetUI<Image>("SubSkullIcon").sprite = player.SubSkullData.subIcon;
        GetUI<Image>("Skill1Icon").sprite = player.Data.skill1Icon;
        GetUI<Image>("Skill2Icon").sprite = player.Data.skill2Icon;
    }
}
