using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BossEntranceScene : BaseScene
{
    [SerializeField] Vector2 playerSpawnPos;
    [SerializeField] GameObject playerUI;
    [SerializeField] CinemachineVirtualCamera vcam;

    public override IEnumerator LoadingRoutine()
    {
        GameObject player = Instantiate(Manager.Game.MainSkullData.skullPrefab, playerSpawnPos, Quaternion.identity);
        Manager.Game.SetPlayer(player.GetComponent<PlayerController>());
        Instantiate(playerUI);
        vcam.m_Follow = player.transform;
        Manager.Game.CreateSmokePools();
        Manager.Sound.StopBGM();
        yield return null;
    }
}
