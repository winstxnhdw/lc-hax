using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Hax;

sealed class Loader : MonoBehaviour {
    const string HarmonyID = "winstxnhdw.lc-hax";

    static GameObject HaxGameObjects { get; } = new("Hax GameObjects");
    static GameObject HaxModules { get; } = new("Hax Modules");
    static bool HasLoaded => Harmony.HasAnyPatches(Loader.HarmonyID);
    static void AddHaxGameObject<T>() where T : Component => Loader.HaxGameObjects.AddComponent<T>();

    static void LoadLibraries() {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (string resourceName in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".dll"))) {
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            _ = AppDomain.CurrentDomain.Load(memoryStream.ToArray());
        }
    }

    internal static void Load() {
        Loader.LoadLibraries();

        if (Loader.HasLoaded) {
            Logger.Write("lc-hax has already loaded!");
            return;
        }

        Loader.LoadHarmonyPatches();
        Loader.LoadHaxModules();
        Loader.LoadHaxGameObjects();
    }

    [Obsolete]
    static void LoadHarmonyPatches() {
        try {
            new Harmony(Loader.HarmonyID).PatchAll();
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
            throw;
        }
    }

    static void LoadHaxGameObjects() {
        DontDestroyOnLoad(Loader.HaxGameObjects);

        Loader.AddHaxGameObject<HaxObjects>();
        Loader.AddHaxGameObject<InputListener>();
        Loader.AddHaxGameObject<ScreenListener>();
        Loader.AddHaxGameObject<GameListener>();
    }

    static void LoadHaxModules() {
        DontDestroyOnLoad(Loader.HaxModules);

        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsDefined(typeof(ModAttribute), inherit: false))
            .ForEach(type => Loader.HaxModules.AddComponent(type));
    }
}
