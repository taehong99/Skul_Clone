using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    const int playerBaseHP = 100;

    PlayerController player;
    public PlayerController Player => player;

    CameraShake shaker;
    public CameraShake Shaker => shaker;

    private int playerHP;
    public int PlayerHP { get { return playerHP; } set { playerHP = value; OnPlayerHPChanged?.Invoke(value); } }
    public UnityAction<int> OnPlayerHPChanged;
    public UnityAction OnPlayerDied;

    public void StartGame()
    {
        Debug.Log("Game started");

        // Player Starting HP
        playerHP = playerBaseHP;

        //Create pools
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/DashSmoke1"), 3, 5);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/JumpSmoke"), 1, 2);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/PlayerHitEffect"), 2, 4);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyHitEffect"), 5, 8);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect"), 5, 8);
    }

    public void SetPlayer(PlayerController player)
    {
        // Assign player to manager at start of every scene
        Debug.Log("Player Assigned");
        this.player = player;
    }

    public void AddCameraShaker()
    {
        // Assign Camera Shaker before boss fight
        shaker = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CameraShake>();
    }

    public void PlayerTakeDamage(int damage)
    {
        PlayerHP -= damage;
        if (PlayerHP <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    public void HandleSkullSwap(PlayerController controller)
    {
        player = controller;
    }
}
