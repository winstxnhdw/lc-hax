using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.Linq;

namespace Hax;

class Loader : MonoBehaviour {
    const string HarmonyID = "winstxnhdw.lc-hax";
    static GameObject HaxGameObjects { get; } = new("Hax GameObjects");
    static GameObject HaxModules { get; } = new("Hax Modules");
    static bool HasLoaded => Harmony.HasAnyPatches(Loader.HarmonyID);
    static void AddHaxModules<T>() where T : Component => Loader.HaxModules.AddComponent<T>();
    static void AddHaxGameObject<T>() where T : Component => Loader.HaxGameObjects.AddComponent<T>();

    static void LoadLibraries() {
        Assembly assembly = Assembly.GetExecutingAssembly();

        ReadOnlySpan<string> resourceNames =
            assembly.GetManifestResourceNames()
                    .Where(name => name.EndsWith(".dll"))
                    .ToArray();

        foreach (string resourceName in resourceNames) {
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

    static void LoadHarmonyPatches() {
        try {
            new Harmony(Loader.HarmonyID).PatchAll();
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
            throw exception;
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
        DontDestroyOnLoad(HaxModules);

        AddHaxModules<ESPMod>();
        AddHaxModules<SaneMod>();
        AddHaxModules<ChatMod>();
        AddHaxModules<FollowMod>();
        AddHaxModules<WeightMod>();
        AddHaxModules<StaminaMod>();
        AddHaxModules<PhantomMod>();
        AddHaxModules<TriggerMod>();
        AddHaxModules<AntiKickMod>();
        AddHaxModules<KillClickMod>();
        AddHaxModules<CrosshairMod>();
        AddHaxModules<MinimalGUIMod>();
        AddHaxModules<PossessionMod>();
        AddHaxModules<DisconnectMod>();
        AddHaxModules<ClearVisionMod>();
        AddHaxModules<InstantInteractMod>();
    }

    internal static void Unload() {
        Destroy(Loader.HaxModules);
        Destroy(Loader.HaxGameObjects);
        new Harmony(Loader.HarmonyID).UnpatchAll();
    }
}
