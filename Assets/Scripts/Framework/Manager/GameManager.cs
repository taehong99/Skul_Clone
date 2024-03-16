using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    PlayerController player;
    public PlayerController Player => player;

    CameraShake shaker;
    public CameraShake Shaker => shaker;

    private int playerHP;
    public int PlayerHP { get { return playerHP; } set { playerHP = value; OnPlayerHPChanged?.Invoke(value); } }
    public UnityAction<int> OnPlayerHPChanged;
    public UnityAction OnPlayerDied;

    private void Start()
    {
        // Assign player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();
        playerHP = player.Data.baseHP;

        // Assign Camera Shaker
        shaker = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CameraShake>();

        //Create pools
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/DashSmoke1"), 3, 5);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/JumpSmoke"), 1, 2);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/PlayerHitEffect"), 2, 4);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyHitEffect"), 5, 8);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect"), 5, 8);
    }

    public void PlayerTakeDamage(int damage)
    {
        PlayerHP -= damage;
        if (PlayerHP <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
