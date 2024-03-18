using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FieldScene : BaseScene
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
        Manager.Game.CreateCombatPools();
        Manager.Game.CreateSmokePools();
        yield return null;
    }
}
