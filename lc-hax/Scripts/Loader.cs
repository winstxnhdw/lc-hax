using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace Hax;

public class Loader : MonoBehaviour {
    static GameObject HaxGameObjects { get; } = new();
    static GameObject HaxModules { get; } = new();

    static void AddHaxModules<T>() where T : Component => Loader.HaxModules.AddComponent<T>();
    static void AddHaxGameObject<T>() where T : Component => Loader.HaxGameObjects.AddComponent<T>();

    static Assembly OnResolveAssembly(object _, ResolveEventArgs args) {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? resourceName =
            assembly.GetManifestResourceNames()
                    .First(name => name.EndsWith($"{new AssemblyName(args.Name).Name}.dll"));

        if (string.IsNullOrWhiteSpace(resourceName)) {
            Logger.Write($"Failed to resolve assembly: {args.Name}");
            throw new FileNotFoundException();
        }

        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);
        return Assembly.Load(memoryStream.ToArray());
    }

    public static void Load() {
        AppDomain.CurrentDomain.AssemblyResolve += Loader.OnResolveAssembly;

        Loader.LoadHarmonyPatches();
        Loader.LoadHaxGameObjects();
        Loader.LoadHaxModules();

        AppDomain.CurrentDomain.AssemblyResolve -= Loader.OnResolveAssembly;
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
        Loader.AddHaxModules<StunMod>();
        Loader.AddHaxModules<ChatMod>();
        Loader.AddHaxModules<FollowMod>();
        Loader.AddHaxModules<WeightMod>();
        Loader.AddHaxModules<StaminaMod>();
        Loader.AddHaxModules<PhantomMod>();
        Loader.AddHaxModules<TriggerMod>();
        Loader.AddHaxModules<AntiKickMod>();
        Loader.AddHaxModules<CrosshairMod>();
        Loader.AddHaxModules<MinimalGUIMod>();
        Loader.AddHaxModules<PossessionMod>();
        Loader.AddHaxModules<ClearVisionMod>();
        Loader.AddHaxModules<InstantInteractMod>();
    }

    public static void Unload() {
        Destroy(Loader.HaxModules);
        Destroy(Loader.HaxGameObjects);
    }
}
