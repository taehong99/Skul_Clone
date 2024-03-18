using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkulLoading : MonoBehaviour
{
    private Image skul;
    [SerializeField] private Sprite[] sprites;

    void Start()
    {
        skul = GetComponent<Image>();
    }

    [SerializeField] float duration;
    private int index;
    private float timer;
    void Update()
    {
        if ((timer += Time.unscaledDeltaTime) >= (duration / sprites.Length))
        {
            timer = 0;
            skul.sprite = sprites[index];
            index = (index + 1) % sprites.Length;
        }
    }
}
