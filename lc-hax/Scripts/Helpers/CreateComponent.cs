using System.Collections.Generic;

namespace Hax;

internal static partial class Helper {
    static Dictionary<string, Queue<Component>> ComponentPools { get; } = [];

    internal static T CreateComponent<T>(string name) where T : Component => new GameObject(name).AddComponent<T>();

    internal static T CreateComponent<T>() where T : Component => new GameObject().AddComponent<T>();

    internal static T CreateComponent<T>(string poolName, int poolSize = 1) where T : Component {
        T gameObject = Helper.CreateComponent<T>(poolName);

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
