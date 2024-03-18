using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : BaseUI
{
    PlayerController player;

    private void Start()
    {
        UpdateIcons();
        //Manager.Events.dataEventDic["skullPickedUp"].OnEventRaised += PickUpSkull;
        Manager.Events.voidEventDic["skullSwapped"].OnEventRaised += UpdateIcons;
    }

    private void OnDisable()
    {
        Manager.Events.voidEventDic["skullSwapped"].OnEventRaised -= UpdateIcons;
    }

    private void Update()
    {
        GetUI<Image>("Skill1Cooldown").fillAmount = Manager.Game.Player.Skill1CooldownRatio;
        GetUI<Image>("Skill2Cooldown").fillAmount = Manager.Game.Player.Skill2CooldownRatio;
        GetUI<Image>("SwapCooldown").fillAmount = Manager.Game.Player.SwapCooldownRatio;
    }

    private void PickUpSkull(PlayerData data)
    {
        GetUI<Image>("MainSkullIcon").sprite = data.mainIcon;
    }

    private void UpdateIcons()
    {
        GetUI<Image>("MainSkullIcon").sprite = Manager.Game.MainSkullData.mainIcon;
        GetUI<Image>("Skill1Icon").sprite = Manager.Game.MainSkullData.skill1Icon;
        GetUI<Image>("Skill2Icon").sprite = Manager.Game.MainSkullData.skill2Icon;

        if (Manager.Game.SubSkullData != null)
        {
            GetUI<Image>("SubSkullIcon").sprite = Manager.Game.SubSkullData.subIcon;
        }
    }
}
