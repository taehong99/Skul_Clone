using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    PlayerController player;
    public PlayerController Player => player;

    private void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        player = go.GetComponent<PlayerController>();
    }

    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
