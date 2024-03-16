using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : PopUpUI
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
        GetUI<Image>("SkullPortrait").sprite = player.Data.portraitIcon;
        GetUI<Image>("SkullPortrait").SetNativeSize();
        GetUI<Image>("SkullPortrait").rectTransform.anchoredPosition = player.Data.portraitPosition;
        GetUI<Image>("Skill1Icon").sprite = player.Data.skill1Icon;
        GetUI<Image>("Skill2Icon").sprite = player.Data.skill2Icon;
    }

    private void UpdateTexts()
    {
        GetUI<TextMeshProUGUI>("SkullName").text = player.Data.skullName;
        GetUI<TextMeshProUGUI>("SkullRarity").text = player.Data.rarity;
        GetUI<TextMeshProUGUI>("SkullType").text = player.Data.type;
        GetUI<TextMeshProUGUI>("SkullIntro").text = player.Data.intro;
        GetUI<TextMeshProUGUI>("SkullDescription").text = player.Data.description;
        GetUI<TextMeshProUGUI>("SwapTitle").text = player.Data.swapTitle;
        GetUI<TextMeshProUGUI>("Skill1Name").text = player.Data.skill1Name;
        GetUI<TextMeshProUGUI>("Skill2Name").text = player.Data.skill2Name;
    }
}
