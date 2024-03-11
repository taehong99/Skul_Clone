using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, VoidEventChannelSO> voidEventDic = new Dictionary<string, VoidEventChannelSO>();
    public Dictionary<string, DirectionEventChannelSO> dirEventDic = new Dictionary<string, DirectionEventChannelSO>();

    private void Start()
    {
        voidEventDic.Add("handSpawned", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/HandSpawnedEvent"));
        voidEventDic.Add("bodySpawned", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/BodySpawnedEvent"));
        voidEventDic.Add("screamFinished", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/ScreamFinishedEvent"));
        voidEventDic.Add("idleFinished", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/IdleFinishedEvent"));
        dirEventDic.Add("sweepPrep", Manager.Resource.Load<DirectionEventChannelSO>("Data/EventChannels/SweepPrepEvent"));
        dirEventDic.Add("sweepReady", Manager.Resource.Load<DirectionEventChannelSO>("Data/EventChannels/SweepReadyEvent"));
    }

    private void OnDestroy()
    {
        foreach (var entry in voidEventDic)
        {
            entry.Value.OnEventRaised = null;
        }
        foreach (var entry in dirEventDic)
        {
            entry.Value.OnEventRaised = null;
        }
    }
}
