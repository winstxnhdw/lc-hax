#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Hax;

static partial class Helper {
    static Dictionary<string, Queue<Component>> ComponentPools { get; } = [];

    static T CreateComponent<T>(string name) where T : Component => new GameObject(name).AddComponent<T>();

    /// <summary>
    ///     Attaches a component to a created game object.
    /// </summary>
    /// <returns>the created component</returns>
    internal static T CreateComponent<T>() where T : Component => new GameObject().AddComponent<T>();

    /// <summary>
    ///     Creates a component and adds it to a queue.
    ///     Used to restrict the total number of existing component(s) belonging to a pool.
    /// </summary>
    /// <returns>the created component</returns>
    internal static T CreateComponent<T>(string poolName, int poolSize = 1) where T : Component {
        T gameObject = CreateComponent<T>(poolName);

        if (!ComponentPools.TryGetValue(poolName, out Queue<Component>? pool)) {
            pool = new Queue<Component>(poolSize);
            ComponentPools[poolName] = pool;
        }

        if (pool.Count == poolSize) Object.Destroy(pool.Dequeue());

        pool.Enqueue(gameObject);

        return gameObject;
    }

    /// <summary>
    ///     Similar to `CreateComponent`, but for components that are implicitly toggleable.
    ///     If the component already exists, no component is created and null is returned.
    /// </summary>
    /// <returns>a component if it has been created, null otherwise</returns>
    internal static T? CreateToggleableComponent<T>(string poolName) where T : Component {
        if (!ComponentPools.TryGetValue(poolName, out Queue<Component>? pool)) return CreateComponent<T>(poolName);

        Object.Destroy(pool.Dequeue());
        return null;
    }
}
