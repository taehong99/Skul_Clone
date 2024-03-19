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

    public AudioClip jumpSFX;
    public AudioClip doubleJumpSFX;

    public AudioClip dashSFX;
    public AudioClip swapSFX;

    public AudioClip teleportSFX;

    [Header("Enemy SFX")]
    public AudioClip hitSFX;

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
}
