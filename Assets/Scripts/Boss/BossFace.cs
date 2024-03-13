using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFace : MonoBehaviour
{
    [SerializeField] Sprite phase2Sprite;
    [SerializeField] Sprite deadSprite;
    SpriteRenderer spriter;

    private void Start()
    {
        spriter = GetComponent<SpriteRenderer>();
        Manager.Events.voidEventDic["phase2Started"].OnEventRaised += TransformP2;
        Manager.Events.voidEventDic["bossDefeated"].OnEventRaised += TransformDead;
    }

    private void TransformP2()
    {
        spriter.sprite = phase2Sprite;
    }

    private void TransformDead()
    {
        spriter.sprite = deadSprite;
    }
}
