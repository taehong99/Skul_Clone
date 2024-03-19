using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AudioClips", fileName = "AudioClips")]
public class AudioClipsSO : ScriptableObject
{
    [Header("BGMs")]
    public AudioClip titleBGM;
    public AudioClip castleBGM;
    public AudioClip fieldBGM;
    public AudioClip bossBGM;

    [Header("Player SFX")]
    public AudioClip attackASFX;
    public AudioClip attackBSFX;
    public AudioClip jumpAttackSFX;
    public AudioClip hitSFX;

    public AudioClip jumpSFX;
    public AudioClip doubleJumpSFX;

    public AudioClip dashSFX;
    public AudioClip swapSFX;

    public AudioClip teleportSFX;

    [Header("Destroyer SFX")]
    public AudioClip heavySlash1;
    public AudioClip heavySlash2;
    public AudioClip heavySlashUp1;
    public AudioClip heavySlashUp2;
    public AudioClip heavySlashDown;
    public AudioClip heavySwoosh;
    public AudioClip bladeHitSFX;

    [Header("Enemy SFX")]
    public AudioClip soldierCry;
    public AudioClip soldierAttack;
    public AudioClip knightCry;
    public AudioClip knightSlam;
    public AudioClip wizardFire;

    [Header("Boss SFX")]
    public AudioClip bossGrabSFX;
    public AudioClip bossSweepSFX;
    public AudioClip bossSlamSFX;
    public AudioClip bossScreamSFX;
    public AudioClip bossRoarSFX;
    public AudioClip bossChargeSFX;
    public AudioClip bossFireSFX;
    public AudioClip bossBreakSFX;
    public AudioClip bossCleanseSFX;

    [Header("UI SFX")]
    public AudioClip pauseUIOpen;
    public AudioClip pauseUIMove;
    public AudioClip inventoryUIOpen;
    public AudioClip inventoryUIClose;
    public AudioClip detailsUIToggle;
}
