using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerController", fileName = "PlayerControllerData")]
public class PlayerControllerDataSO : ScriptableObject
{
    [Header("Player Movement")]
    public float moveSpeed = 5f;

    [Header("Player Jump")]
    public float jumpPower = 10f;
    public int jumpCount = 2;
    public float fallMultiplier = 2.5f;
    public float coyoteTime;

    [Header("Player Dash")]
    public int dashCount = 2;
    public float dashPower = 18f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Player Interact")]
    public float interactRange = 1f;

    [Header("Player Combat")]
    public int baseDamage = 10;

    [Header("Layers")]
    public LayerMask playerMask;
    public LayerMask groundMask;
    public LayerMask interactableMask;
    public LayerMask hittableMask;
}
