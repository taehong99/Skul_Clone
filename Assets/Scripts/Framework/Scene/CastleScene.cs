using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScene : BaseScene
{
    [SerializeField] GameObject player;
    [SerializeField] Vector2 playerSpawnPos;
    [SerializeField] GameObject playerUI;
    [SerializeField] CinemachineVirtualCamera vcam;

    private void Start()
    {
        Manager.Game.StartGame();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Triggered");
        Manager.Scene.LoadScene("2.CastleLobby");
    }

    public override IEnumerator LoadingRoutine()
    {
        player = Instantiate(player, playerSpawnPos, Quaternion.identity);
        Manager.Game.SetPlayer(player.GetComponent<PlayerController>());
        Instantiate(playerUI);
        vcam.m_Follow = player.transform;
        yield return null;
    }
}
