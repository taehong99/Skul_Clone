using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlatforms : MonoBehaviour
{
    BoxCollider2D[] platforms;

    private void Awake()
    {
        platforms = GetComponentsInChildren<BoxCollider2D>();
    }

    public void SetPlatforms(bool b)
    {
        foreach(var platform in platforms)
        {
            platform.enabled = b;
        }
    }
}
