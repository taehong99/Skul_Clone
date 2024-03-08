using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LedgeChecker : MonoBehaviour
{
    public UnityAction OnReachedEndOfLedge;

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("REACHED END OF LEDGE");
        OnReachedEndOfLedge?.Invoke();
    }
}
