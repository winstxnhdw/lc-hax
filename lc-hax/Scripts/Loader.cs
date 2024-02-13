using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.Linq;

namespace Hax;

internal class Loader : MonoBehaviour {
    static GameObject HaxGameObjects { get; } = new("Hax GameObjects");
    static GameObject HaxModules { get; } = new("Hax Modules");

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
        Loader.LoadHarmonyPatches();
        Loader.LoadHaxGameObjects();
        Loader.LoadHaxModules();
    }

    static void LoadHarmonyPatches() {
        try {
            new Harmony("winstxnhdw.lc-hax").PatchAll();
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
        DontDestroyOnLoad(Loader.HaxModules);

        Loader.AddHaxModules<ESPMod>();
        Loader.AddHaxModules<SaneMod>();
        Loader.AddHaxModules<ChatMod>();
        Loader.AddHaxModules<FollowMod>();
        Loader.AddHaxModules<WeightMod>();
        Loader.AddHaxModules<StaminaMod>();
        Loader.AddHaxModules<PhantomMod>();
        Loader.AddHaxModules<TriggerMod>();
        Loader.AddHaxModules<AntiKickMod>();
        Loader.AddHaxModules<StunClickMod>();
        Loader.AddHaxModules<KillClickMod>();
        Loader.AddHaxModules<CrosshairMod>();
        Loader.AddHaxModules<MinimalGUIMod>();
        Loader.AddHaxModules<PossessionMod>();
        Loader.AddHaxModules<DisconnectMod>();
        Loader.AddHaxModules<ClearVisionMod>();
        Loader.AddHaxModules<InstantInteractMod>();
    }

    internal static void Unload() {
        Destroy(Loader.HaxModules);
        Destroy(Loader.HaxGameObjects);
    }
}
