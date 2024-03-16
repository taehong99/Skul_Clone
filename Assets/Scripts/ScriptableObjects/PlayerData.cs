using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player", fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Stats")]
    public int baseHP;
    public int baseDamage;
    public int attackRange;
    public float moveSpeed;
    public int dashCount;

    [Header("Skull-Specific Stats")]
    public RuntimeAnimatorController animatorController;
    public float skill1Cooldown;
    public float skill2Cooldown;

    [Header("Icons")]
    public Sprite skullIcon;
    public Sprite skill1Icon;
    public Sprite skill2Icon;

    [Header("Texts")]
    public string skullName;
    public string type;
    public string rarity;
    public string swapTitle;
    public string swapDescription;
    public string skill1Name;
    public string skill1Description;
    public string skill2Name;
    public string skill2Description;
}
