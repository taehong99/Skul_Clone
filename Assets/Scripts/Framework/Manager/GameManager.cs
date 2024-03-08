using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    PlayerController player;
    public PlayerController Player => player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();

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
