#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

static class Finder {
    internal static List<GameObject> RootSceneObjects => SceneManager.GetActiveScene().GetRootGameObjects().ToList();

    /// <summary>
    ///     This bypasses Also the Inactive Object bug that Unity has when using .find being unable to find inactive objects.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal static GameObject? Find(string path, bool dontWarn = true) {
        if (path.IsNotNullOrEmptyOrWhiteSpace()) {
            // Get all paths
            string[] paths = path.Split('/');
            // Get the root path
            string RootPath = paths[0];
            // get the rest of the child path minus the root
            string[] ChildPaths = paths.Skip(1).ToArray();
            // Get the root gameobject
            GameObject? Root = GameObject.Find(RootPath);
            // If the root gameobject is null, return null
            if (Root == null) {
                Root = FindRootSceneObject(RootPath);
                if (Root == null) {
                    if (!dontWarn)
                        Logger.Write(
                            $"[ERROR (Find) ]  Gameobject on path [ {path} ]  is Invalid, No Root Object Found!");
                    return null;
                }
            }

            if (ChildPaths.Length == 0) return Root;
            // convert the rest of the childs into a string
            string ChildPath = string.Join("/", ChildPaths);
            // Find the child gameobject
            GameObject? result = Root.FindObject(ChildPath, true);
            if (result == null) {
                if (!dontWarn)
                    Logger.Write($"[ERROR (Find) ]  Gameobject on path [ {path} ]  is Invalid, No Child Object Found!");
                return null;
            }

            return result;
        }

        return null;
    }

    public static List<T>? GetRootGameObjectsComponents<T>(bool includeInactive = true) where T : Component {
        try {
            List<T> results = new();
            for (int i = 0; i < RootSceneObjects.Count; i++) {
                GameObject? obj = RootSceneObjects[i];
                T[]? objects = obj.GetComponentsInChildren<T>(includeInactive);
                if (objects.Count() != 0)
                    for (int i1 = 0; i1 < objects.Count(); i1++) {
                        T? component = objects[i1];
                        if (!results.Contains(component)) results.Add(component);
                    }
            }

            return results;
        }
        catch (Exception e) {
            Logger.Write("Error parsing Components from Root Objects");
            Logger.Write(e);
            return null;
        }
    }

    internal static GameObject? FindRootSceneObject(string name, bool dontWarn = true) {
        GameObject[] list = SceneManager.GetActiveScene().GetRootGameObjects();

        for (int i = 0; i < list.Count(); i++) {
            GameObject? obj = list[i];
            if (obj != null && obj.name.Equals(name)) return obj;
        }

        if (!dontWarn)
            Logger.Write(
                $"[WARNING (FindRootSceneObject) ]  Root Gameobject name [ {name} ]  is Invalid, No Object Found!");
        return null;
    }


    internal static GameObject? FindObject(this GameObject gameobject, string path, bool dontWarn = false) {
        if (gameobject == null) return null;
        Transform? obj = gameobject.transform.Find(path);
        if (obj == null) {
            if (!dontWarn)
                Logger.Write(
                    $"[WARNING (FindObject) ]  Transform {gameobject.name} Doesnt have a object in path [ {path} ] !");
            return null;
        }

        return obj.gameObject;
    }


    internal static Transform? FindObject(this Transform transform, string path, bool dontWarn = false) {
        if (transform == null) return null;
        Transform? obj = transform.Find(path);
        if (obj == null) {
            if (!dontWarn)
                Logger.Write(
                    $"[WARNING (FindObject) ]  Transform {transform.name} Doesnt have a object in path [ {path} ] !");
            return null;
        }

        return obj;
    }

    // recurse method to return full path of a transform
    internal static string GetPath(this Transform transform) {
        if (transform == null) return string.Empty;
        return transform.parent == null ? transform.name : transform.parent.GetPath() + "/" + transform.name;
    }

}
