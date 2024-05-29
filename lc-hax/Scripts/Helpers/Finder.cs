using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class Finder
{
    internal static List<GameObject> RootSceneObjects => SceneManager.GetActiveScene().GetRootGameObjects().ToList();

    /// <summary>
    ///     This bypasses Also the Inactive Object bug that Unity has when using .find being unable to find inactive objects.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal static GameObject? Find(string path, bool dontWarn = true)
    {
        if (path.IsNotNullOrEmptyOrWhiteSpace())
        {
            // Get all paths
            string[] paths = path.Split('/');
            // Get the root path
            var RootPath = paths[0];
            // get the rest of the child path minus the root
            var ChildPaths = paths.Skip(1).ToArray();
            // Get the root gameobject
            var Root = GameObject.Find(RootPath);
            // If the root gameobject is null, return null
            if (Root == null)
            {
                Root = FindRootSceneObject(RootPath);
                if (Root == null)
                {
                    if (!dontWarn)
                        Logger.Write(
                            $"[ERROR (Find) ]  Gameobject on path [ {path} ]  is Invalid, No Root Object Found!");
                    return null;
                }
            }

            if (ChildPaths.Length == 0) return Root;
            // convert the rest of the childs into a string
            var ChildPath = string.Join("/", ChildPaths);
            // Find the child gameobject
            var result = Root.FindObject(ChildPath, true);
            if (result == null)
            {
                if (!dontWarn)
                    Logger.Write($"[ERROR (Find) ]  Gameobject on path [ {path} ]  is Invalid, No Child Object Found!");
                return null;
            }

            return result;
        }

        return null;
    }

    public static List<T>? GetRootGameObjectsComponents<T>(bool includeInactive = true) where T : Component
    {
        try
        {
            List<T> results = new();
            for (var i = 0; i < RootSceneObjects.Count; i++)
            {
                var obj = RootSceneObjects[i];
                var objects = obj.GetComponentsInChildren<T>(includeInactive);
                if (objects.Count() != 0)
                    for (var i1 = 0; i1 < objects.Count(); i1++)
                    {
                        var component = objects[i1];
                        if (!results.Contains(component)) results.Add(component);
                    }
            }

            return results;
        }
        catch (Exception e)
        {
            Logger.Write("Error parsing Components from Root Objects");
            Logger.Write(e);
            return null;
        }
    }

    internal static GameObject? FindRootSceneObject(string name, bool dontWarn = true)
    {
        GameObject[] list = SceneManager.GetActiveScene().GetRootGameObjects();

        for (var i = 0; i < list.Count(); i++)
        {
            var obj = list[i];
            if (obj != null && obj.name.Equals(name)) return obj;
        }

        if (!dontWarn)
            Logger.Write(
                $"[WARNING (FindRootSceneObject) ]  Root Gameobject name [ {name} ]  is Invalid, No Object Found!");
        return null;
    }


    internal static GameObject? FindObject(this GameObject gameobject, string path, bool dontWarn = false)
    {
        if (gameobject == null) return null;
        var obj = gameobject.transform.Find(path);
        if (obj == null)
        {
            if (!dontWarn)
                Logger.Write(
                    $"[WARNING (FindObject) ]  Transform {gameobject.name} Doesnt have a object in path [ {path} ] !");
            return null;
        }

        return obj.gameObject;
    }


    internal static Transform? FindObject(this Transform transform, string path, bool dontWarn = false)
    {
        if (transform == null) return null;
        var obj = transform.Find(path);
        if (obj == null)
        {
            if (!dontWarn)
                Logger.Write(
                    $"[WARNING (FindObject) ]  Transform {transform.name} Doesnt have a object in path [ {path} ] !");
            return null;
        }

        return obj;
    }
}