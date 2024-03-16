using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player", fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Stats")]
    public float damageMultiplier;
    public float attackRange;
    public int dashCount;

    [Header("Skull-Specific Stats")]
    public GameObject skullPrefab;
    public RuntimeAnimatorController animatorController;
    public float skill1Cooldown;
    public float skill2Cooldown;

    [Header("Icons")]
    public Sprite portraitIcon;
    public Vector2 portraitPosition;
    public Sprite mainIcon;
    public Sprite subIcon;
    public Sprite skill1Icon;
    public Sprite skill2Icon;

    [Header("Texts")]
    public string skullName;
    public string type;
    public string rarity;

    [TextArea(3, 10)]
    public string intro;
    [TextArea(3, 10)]
    public string description;

    public string skill1Name;
    public string skill2Name;
    public string swapTitle;

    [TextArea(3, 10)]
    public string skill1Description;
    [TextArea(3, 10)]
    public string skill2Description;
    [TextArea(3, 10)]
    public string swapDescription;

    
}
