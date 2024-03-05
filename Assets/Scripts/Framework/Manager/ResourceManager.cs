using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> resources = new Dictionary<string, Object>();

    public T Load<T>(string path) where T : Object
    {
        string key = $"{path}_{typeof(T)}";

        if (resources.TryGetValue(key, out Object obj))
        {
            return obj as T;
        }
        else
        {
            T resource = Resources.Load<T>(path);
            resources.Add(key, resource);
            return resource;
        }
    }
}
