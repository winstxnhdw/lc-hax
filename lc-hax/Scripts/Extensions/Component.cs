using System.Linq;
using UnityEngine;

static partial class Extensions {
    #region GameObject

    internal static T? GetGetInChildrens_OrAddComponent<T>(this GameObject obj, bool includeInactive = false) where T : Component {
        if (obj == null) return null;
        var result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        if (result == null)
            result = obj.AddComponent<T>();
        return result;
    }

    internal static T? GetGetInChildrens<T>(this GameObject obj, bool includeInactive = false) where T : Component {
        if (obj == null) return null;
        var result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        return result;
    }

    internal static T? GetGetInChildrens_OrParent<T>(this GameObject obj, bool includeInactive = false) where T : Component {
        if (obj == null) return null;
        var result = obj.GetComponent<T>();
        if (result == null)
            result = obj.GetComponentInChildren<T>(includeInactive);
        if (result == null)
            result = obj.GetComponentInParent<T>();
        return result;
    }

    internal static T? GetOrAddComponent<T>(this GameObject obj) where T : Component {
        if (obj == null) return null;
        T? result = obj.GetComponent<T>() ?? obj.AddComponent<T>();
        return result;
    }

    internal static void RemoveComponent<T>(this GameObject obj) where T : Component {
        if (obj != null) {
            T existing = obj.GetComponent<T>();
            if (existing) {
                UnityEngine.Object.Destroy(existing);
            }
        }
    }

    internal static void RemoveComponents<T>(this GameObject parent) where T : Component {
        if (parent == null) return;
        T[] ParentComp = parent.GetComponents<T>();
        if (ParentComp != null) {
            if (ParentComp.Count() != 0) {
                foreach (T comp in ParentComp) {
                    if (comp != null) {
                        UnityEngine.Object.Destroy(comp);
                    }
                }
            }
        }
        T[] childs1 = parent.GetComponentsInChildren<T>(true);
        if (childs1 != null) {
            if (childs1.Count() != 0) {
                foreach (T comp in childs1) {
                    if (comp != null) {
                        UnityEngine.Object.Destroy(comp);
                    }
                }
            }
        }
    }

    public static void SetComponentState<T>(this GameObject parent, bool enabled) where T : Behaviour {
        if (parent == null) return;
        T[] ParentComp = parent.GetComponents<T>();
        if (ParentComp != null) {
            if (ParentComp.Count() != 0) {
                foreach (T comp in ParentComp) {
                    if (comp != null) {
                        comp.enabled = enabled;
                    }
                }
            }
        }
    }

    internal static bool HasComponent<T>(this GameObject obj) where T : Component {
        if (obj != null) {
            T existing = obj.GetComponent<T>();
            if (existing) {
                return true;
            }
        }
        return false;
    }

    #endregion GameObject

    #region Transform

    internal static T? GetGetInChildrens_OrAddComponent<T>(this Transform obj, bool includeInactive = false) where T : Component => obj.gameObject.GetGetInChildrens_OrAddComponent<T>(includeInactive);

    internal static T? GetGetInChildrens<T>(this Transform obj, bool includeInactive = false) where T : Component => obj.gameObject.GetGetInChildrens<T>(includeInactive);

    internal static T? GetGetInChildrens_OrParent<T>(this Transform obj, bool includeInactive = false) where T : Component => obj.gameObject.GetGetInChildrens_OrParent<T>(includeInactive);

    internal static T? GetOrAddComponent<T>(this Transform obj) where T : Component => obj.gameObject.GetOrAddComponent<T>();

    internal static void RemoveComponent<T>(this Transform obj) where T : Component => obj.gameObject.RemoveComponent<T>();

    internal static void RemoveComponents<T>(this Transform obj) where T : Component => obj.gameObject.RemoveComponents<T>();

    internal static bool HasComponent<T>(this Transform obj) where T : Component => obj.gameObject.HasComponent<T>();

    #endregion Transform

    #region component

    internal static T AddComponent<T>(this Component c) where T : Component => c.gameObject.AddComponent<T>();

    internal static T? GetGetInChildrens_OrAddComponent<T>(this Component obj, bool includeInactive = false) where T : Component => obj.transform.GetGetInChildrens_OrAddComponent<T>(includeInactive);

    internal static T? GetGetInChildrens<T>(this Component obj, bool includeInactive = false) where T : Component => obj.transform.GetGetInChildrens<T>(includeInactive);

    internal static T? GetGetInChildrens_OrParent<T>(this Component obj, bool includeInactive = false) where T : Component => obj.transform.GetGetInChildrens_OrParent<T>(includeInactive);

    internal static T? GetOrAddComponent<T>(this Component obj) where T : Component => obj.transform.GetOrAddComponent<T>();

    internal static void RemoveComponent<T>(this Component obj) where T : Component => obj.transform.RemoveComponent<T>();

    internal static void RemoveComponents<T>(this Component obj) where T : Component => obj.transform.RemoveComponents<T>();

    internal static bool HasComponent<T>(this Component obj) where T : Component => obj.transform.HasComponent<T>();

    #endregion component

}
