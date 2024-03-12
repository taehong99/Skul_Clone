using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJaw : MonoBehaviour
{
    [SerializeField] Sprite phase2Sprite;
    SpriteRenderer spriter;

    private void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        Manager.Events.voidEventDic["phase2Started"].OnEventRaised += Transform;
    }

    public void Transform()
    {
        spriter.sprite = phase2Sprite;
    }
}
