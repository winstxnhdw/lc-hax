using System.Collections.Generic;
using Dissonance;
using UnityEngine;

static class ComponentUtils {
    #region GameObject

    internal static T? GetGetInChildrens_OrAddComponent<T>(this GameObject obj, bool includeInactive = false)
        where T : Component {
        if (obj == null) return null;
        T? result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        if (result == null)
            result = obj.AddComponent<T>();
        return result;
    }

    internal static T? GetGetInChildrens<T>(this GameObject obj, bool includeInactive = false) where T : Component {
        if (obj == null) return null;
        T? result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        return result;
    }

    internal static T? GetGetInChildrens_OrParent<T>(this GameObject obj, bool includeInactive = false)
        where T : Component {
        if (obj == null) return null;
        T? result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        if (result == null)
            result = obj.GetComponentInParent<T>();
        return result;
    }

    internal static T? GetOrAddComponent<T>(this GameObject obj) where T : Component {
        if (obj == null) return null;
        T? result = obj.GetComponent<T>();
        if (result == null)
            result = obj.AddComponent<T>();
        return result;
    }

    internal static void RemoveComponent<T>(this GameObject obj) where T : Component {
        if (obj != null) {
            T? existing = obj.GetComponent<T>();
            if (existing) Object.Destroy(existing);
        }
    }

    internal static void RemoveComponents<T>(this GameObject parent) where T : Component {
        if (parent == null) return;
        T[] parentComp = parent.GetComponents<T>();
        if (parentComp != null)
            if (parentComp.Length != 0)
                foreach (T? comp in parentComp)
                    if (comp != null)
                        Object.Destroy(comp);

        T[]? childs1 = parent.GetComponentsInChildren<T>(true);
        if (childs1 != null)
            if (childs1.Length != 0)
                foreach (T? comp in childs1)
                    if (comp != null)
                        Object.Destroy(comp);
    }

    public static void SetComponentState<T>(this GameObject parent, bool enabled) where T : Behaviour {
        if (parent == null) return;
        T[]? parentComp = parent.GetComponents<T>();
        if (parentComp != null)
            if (parentComp.Length != 0)
                foreach (T? comp in parentComp)
                    if (comp != null)
                        comp.enabled = enabled;
    }

    public static void ActivateComponents<T>(this GameObject parent) where T : Behaviour {
        if (parent == null) return;
        T[]? parentComp = parent.GetComponents<T>();
        if (parentComp != null)
            if (parentComp.Length != 0)
                foreach (T? comp in parentComp)
                    if (comp != null)
                        comp.enabled = true;
        HashSet<Transform>? childs = parent.transform.Get_All_Childs();
        if (childs == null) return;
        foreach (Transform child in childs) {
            T[] comps = child.GetComponents<T>();
            if (comps == null) continue;
            if (comps.Length != 0) continue;
            foreach (T comp in comps)
                if (comp != null)
                    comp.enabled = true;
        }
    }

    internal static bool HasComponent<T>(this GameObject obj) where T : Component {
        if (obj != null) {
            T? existing = obj.GetComponent<T>();
            if (existing) return true;
        }

        return false;
    }

    #endregion GameObject

    #region Transform

    internal static T? GetGetInChildrens_OrAddComponent<T>(this Transform obj, bool includeInactive = false)
        where T : Component =>
        obj.gameObject.GetGetInChildrens_OrAddComponent<T>(includeInactive);

    internal static T? GetGetInChildrens<T>(this Transform obj, bool includeInactive = false) where T : Component =>
        obj.gameObject.GetGetInChildrens<T>(includeInactive);

    internal static T? GetGetInChildrens_OrParent<T>(this Transform obj, bool includeInactive = false)
        where T : Component =>
        obj.gameObject.GetGetInChildrens_OrParent<T>(includeInactive);

    internal static T? GetOrAddComponent<T>(this Transform obj) where T : Component =>
        obj.gameObject.GetOrAddComponent<T>();

    internal static void RemoveComponent<T>(this Transform obj) where T : Component =>
        obj.gameObject.RemoveComponent<T>();

    internal static void RemoveComponents<T>(this Transform obj) where T : Component =>
        obj.gameObject.RemoveComponents<T>();

    internal static bool HasComponent<T>(this Transform obj) where T : Component => obj.gameObject.HasComponent<T>();

    #endregion Transform

    #region component

    internal static T AddComponent<T>(this Component c) where T : Component => c.gameObject.AddComponent<T>();

    internal static T? GetGetInChildrens_OrAddComponent<T>(this Component obj, bool includeInactive = false)
        where T : Component =>
        obj.transform.GetGetInChildrens_OrAddComponent<T>(includeInactive);

    internal static T? GetGetInChildrens<T>(this Component obj, bool includeInactive = false) where T : Component =>
        obj.transform.GetGetInChildrens<T>(includeInactive);

    internal static T? GetGetInChildrens_OrParent<T>(this Component obj, bool includeInactive = false)
        where T : Component =>
        obj.transform.GetGetInChildrens_OrParent<T>(includeInactive);

    internal static T? GetOrAddComponent<T>(this Component obj) where T : Component =>
        obj.transform.GetOrAddComponent<T>();

    internal static void RemoveComponent<T>(this Component obj) where T : Component =>
        obj.transform.RemoveComponent<T>();

    internal static void RemoveComponents<T>(this Component obj) where T : Component =>
        obj.transform.RemoveComponents<T>();

    internal static bool HasComponent<T>(this Component obj) where T : Component => obj.transform.HasComponent<T>();

    #endregion component

    #region Childs Parser

    internal static HashSet<Transform> Get_Childs(this GameObject obj) => obj.transform.Get_Childs();

    internal static HashSet<Transform> Get_Childs(this Transform obj) {
        HashSet<Transform> childs = new();
        for (int i = 0; i < obj.childCount; i++) {
            Transform item = obj.GetChild(i);
            if (item != null) {
                _ = childs.Add(item);
            }
        }

        return childs;
    }

    internal static HashSet<Transform>? Get_All_Childs(this Transform item) {
        CheckTransform(item);
        return Transforms;
    }


    private static void CheckTransform(Transform transform) {
        Transforms = [];

        if (transform == null) {
            Logger.Write("Debug: CheckTransform transform is null");
            return;
        }

        GetChildren(transform);
    }

    private static void GetChildren(Transform transform) {
        if (Transforms != null) _ = Transforms.Add(transform);
        for (int i = 0; i < transform.childCount; i++) {
            GetChildren(transform.GetChild(i));
        }
    }

    private static HashSet<Transform>? Transforms;


    #endregion
}
