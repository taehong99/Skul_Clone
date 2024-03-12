using Cinemachine;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    PlayerController player;
    public PlayerController Player => player;
    CameraShake shaker;
    public CameraShake Shaker => shaker;

    private void Start()
    {
        // Assign player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();

        // Assign Camera Shaker
        shaker = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CameraShake>();

        //Create pools
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/DashSmoke1"), 3, 5);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/JumpSmoke"), 1, 2);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/PlayerHitEffect"), 2, 4);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyHitEffect"), 5, 8);
        Manager.Pool.CreatePool(Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect"), 5, 8);
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
