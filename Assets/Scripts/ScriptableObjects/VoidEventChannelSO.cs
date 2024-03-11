using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "VoidEventChannel")]
public class VoidEventChannelSO : ScriptableObject
{
    [Tooltip("The action to perform")]
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }
}
