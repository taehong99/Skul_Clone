using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShinies : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodyShiny;
    [SerializeField] SpriteRenderer jawShiny;

    public IEnumerator TurnOn()
    {
        float t = 0;
        while (t <= 3)
        {
            Color newColor = bodyShiny.color;
            newColor.a = t / 3;
            bodyShiny.color = newColor;
            jawShiny.color = newColor;
            t += Time.deltaTime;
            yield return null;
        }
    }

    public void TurnOff()
    {
        Color newColor = bodyShiny.color;
        newColor.a = 0;
        bodyShiny.color = newColor;
        jawShiny.color = newColor;
    }
}
