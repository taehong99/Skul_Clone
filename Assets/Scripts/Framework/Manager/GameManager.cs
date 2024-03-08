using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    PlayerController player;
    public PlayerController Player => player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerController>();
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
