using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace Hax;

public class Loader : MonoBehaviour {
    public static string HarmonyPatchName { get; } = "winstxnhdw.lc-hax";
    public static string HaxGameObjectsName { get; } = "Hax GameObjects";
    public static string HaxModulesName { get; } = "Hax Modules";

    static GameObject HaxGameObjects { get; } = new(HaxGameObjectsName);
    static GameObject HaxModules { get; } = new(HaxModulesName);

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
            if (!Harmony.HasAnyPatches(HarmonyPatchName)) {
                new Harmony(HarmonyPatchName).PatchAll();
            }
            else {
                Logger.Write("Harmony patches already loaded.");
            }
                
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
            throw exception;
        }
    }

    static void LoadHaxGameObjects() {
        if (GameObject.Find(HaxGameObjectsName)) {
            Logger.Write("Hax GameObjects already loaded.");
        }

        DontDestroyOnLoad(Loader.HaxGameObjects);

        Loader.AddHaxGameObject<HaxObjects>();
        Loader.AddHaxGameObject<InputListener>();
        Loader.AddHaxGameObject<ScreenListener>();
        Loader.AddHaxGameObject<GameListener>();
    }

    static void LoadHaxModules() {

        if (GameObject.Find(HaxModulesName)) return;

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
        Loader.AddHaxModules<DisconnectMod>();
        Loader.AddHaxModules<ClearVisionMod>();
        Loader.AddHaxModules<InstantInteractMod>();

        // Dont Disable this line, it is used to fill the Helper in case is injected during a full loaded level.
        Loader.AddHaxModules<RefreshMod>();


    }

    public static void Unload() {
        Destroy(Loader.HaxModules);
        Destroy(Loader.HaxGameObjects);
    }
}
