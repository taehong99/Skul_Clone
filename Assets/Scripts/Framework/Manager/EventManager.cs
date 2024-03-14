using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<string, VoidEventChannelSO> voidEventDic = new Dictionary<string, VoidEventChannelSO>();
    public Dictionary<string, DirectionEventChannelSO> dirEventDic = new Dictionary<string, DirectionEventChannelSO>();

    private void Start()
    {
        // boss events
        voidEventDic.Add("phase2Started", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/Phase2StartedEvent"));
        voidEventDic.Add("whiteFlash", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/WhiteFlashEvent"));
        voidEventDic.Add("bossDefeated", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/BossDefeatedEvent"));

        // enemy events
        voidEventDic.Add("enemySpawned", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/EnemySpawnedEvent"));
        voidEventDic.Add("enemyKilled", Manager.Resource.Load<VoidEventChannelSO>("Data/EventChannels/EnemyKilledEvent"));
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
