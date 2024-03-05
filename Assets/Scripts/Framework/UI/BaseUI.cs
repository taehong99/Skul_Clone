using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<string, Component> componentDic;

    protected virtual void Awake()
    {
        Bind();
    }

    private void Bind()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length * 4);
        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd($"{child.gameObject.name}", child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<string, Component>(components.Length * 4);
        foreach (Component child in components)
        {
            componentDic.TryAdd($"{child.gameObject.name}_{components.GetType().Name}", child);
        }
    }

    public GameObject GetUI(string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

    public T GetUI<T>(string name) where T : Component
    {
        componentDic.TryGetValue($"{name}_{typeof(T).Name}", out Component component);
        if (component != null)
            return component as T;

        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return null;

        component = gameObject.GetComponent<T>();
        if (component == null)
            return null;

        componentDic.TryAdd($"{name}_{typeof(T).Name}", component);
        return component as T;
    }
}
