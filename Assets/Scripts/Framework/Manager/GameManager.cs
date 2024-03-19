using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public const int playerBaseHP = 100;
    [SerializeField] GameObject skul;
    [SerializeField] GameObject destroyer;

    PlayerController player;
    public PlayerController Player => player;
    private PlayerData mainSkullData;
    private PlayerData subSkullData;
    public PlayerData MainSkullData => mainSkullData;
    public PlayerData SubSkullData => subSkullData;

    private int playerHP;
    public int PlayerHP { get { return playerHP; } set { playerHP = value; OnPlayerHPChanged?.Invoke(value); } }
    public UnityAction<int> OnPlayerHPChanged;
    public UnityAction OnPlayerDied;

    CameraShake shaker;
    public CameraShake Shaker => shaker;

    public void StartGame() // Only called once at start of game
    {
        Debug.Log("Game started");
        playerHP = playerBaseHP;
        mainSkullData = skul.GetComponent<PlayerController>().Data;
        subSkullData = null;

        //Create pools
        CreateSmokePools();
        SetEvents();
    }

    public void SetEvents()
    {
        Manager.Events.dataEventDic["skullPickedUp"].OnEventRaised += PickUpSkull;
    }

    public void CreateSmokePools()
    {
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/DashSmoke1"), 3, 5);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/JumpSmoke"), 1, 2);
    }

    public void CreateCombatPools() // Only called in maps with enemies
    {
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/PlayerHitEffect"), 2, 4);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyHitEffect"), 5, 8);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect"), 5, 8);
    }

    public void SetPlayer(PlayerController player) // Assign player to manager at start of every scene
    {
        this.player = player;
    }

    public void ChangeVcamTarget()
    {
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = player.transform;
    }

    public void AddCameraShaker(CinemachineVirtualCamera vcam) // Assign Camera Shaker for boss fight
    {
        shaker = vcam.GetComponent<CameraShake>();
    }

    public void PlayerTakeDamage(int damage)
    {
        PlayerHP -= damage;
        if (PlayerHP <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    private void PickUpSkull(PlayerData skullData)
    {
        Debug.Log("Picked UP");
        subSkullData = skullData;
        SwapSkull();
    }

    public void SwapSkull()
    {
        // Update skull data
        PlayerData temp = mainSkullData;
        mainSkullData = subSkullData;
        subSkullData = temp;

        // Destroy old skull then instantiate and assign new skull
        Vector3 spawnPos = player.transform.position;
        Quaternion spawnRot = player.transform.rotation;
        Destroy(player.gameObject);
        player = Instantiate(mainSkullData.skullPrefab, spawnPos, spawnRot).GetComponent<PlayerController>();
        SetPlayer(player);
        ChangeVcamTarget();
        //player.SwapEffect();
        Manager.Events.voidEventDic["skullSwapped"].RaiseEvent();
    }
}
