using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : PlayerController
{
    [Header("Skul Swap Effect")] // Slows time for everything except himself
    [SerializeField] float slowAmount;
    [SerializeField] float slowDuration;
}
