using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    static Dictionary<string, Queue<Component>> ComponentPools { get; } = [];

    public static T CreateComponent<T>() where T : Component => new GameObject().AddComponent<T>();

    public static T CreateComponent<T>(string poolName, int poolSize = 1) where T : Component {
        T gameObject = Helper.CreateComponent<T>();

        if (!Helper.ComponentPools.TryGetValue(poolName, out Queue<Component> pool)) {
            pool = new Queue<Component>(poolSize);
            Helper.ComponentPools[poolName] = pool;
        }

        if (pool.Count == poolSize) {
            GameObject.Destroy(pool.Dequeue());
        }

        pool.Enqueue(gameObject);

        return gameObject;
    }
}
