using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionUI : PopUpUI
{
    PlayerController player;

    private void Start()
    {
        player = Manager.Game.Player;
        UpdateIcons();
        UpdateTexts();
    }

    private void UpdateIcons()
    {
        GetUI<Image>("Skill1Icon").sprite = player.Data.skill1Icon;
        GetUI<Image>("Skill2Icon").sprite = player.Data.skill2Icon;
    }

    private void UpdateTexts()
    {
        GetUI<TextMeshProUGUI>("SwapTitle").text = player.Data.swapTitle;
        GetUI<TextMeshProUGUI>("SwapDescription").text = player.Data.swapDescription;
        GetUI<TextMeshProUGUI>("Skill1Name").text = player.Data.skill1Name;
        GetUI<TextMeshProUGUI>("Skill1Cooldown").text = player.Data.skill1Cooldown.ToString();
        GetUI<TextMeshProUGUI>("Skill1Description").text = player.Data.skill1Description;
        GetUI<TextMeshProUGUI>("Skill2Name").text = player.Data.skill2Name;
        GetUI<TextMeshProUGUI>("Skill2Cooldown").text = player.Data.skill2Cooldown.ToString();
        GetUI<TextMeshProUGUI>("Skill2Description").text = player.Data.skill2Description;
    }
}
