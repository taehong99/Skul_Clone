using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}
