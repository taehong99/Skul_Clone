using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LobbyScene : BaseScene
{
    [SerializeField] GameObject player;
    [SerializeField] Vector2 playerSpawnPos;
    [SerializeField] GameObject playerUI;
    [SerializeField] CinemachineVirtualCamera vcam;

    public override IEnumerator LoadingRoutine()
    {
        player = Instantiate(player, playerSpawnPos, Quaternion.identity);
        Manager.Game.SetPlayer(player.GetComponent<PlayerController>());
        Instantiate(playerUI);
        vcam.ForceCameraPosition(player.transform.position, Quaternion.identity);
        vcam.m_Follow = player.transform;
        Manager.Game.CreateSmokePools();
        Manager.Sound.PlayBGM(Manager.Sound.Data.fieldBGM);
        yield return null;
    }
}
