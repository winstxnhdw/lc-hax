using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Hax;

internal class Loader : MonoBehaviour
{
    private const string HarmonyID = "winstxnhdw.lc-hax";

    private static GameObject HaxGameObjects { get; } = new("Hax GameObjects");
    private static GameObject HaxModules { get; } = new("Hax Modules");

    private static bool HasLoaded => Harmony.HasAnyPatches(HarmonyID);

    private static void AddHaxModules<T>() where T : Component
    {
        HaxModules.AddComponent<T>();
    }

    private static void AddHaxGameObject<T>() where T : Component
    {
        HaxGameObjects.AddComponent<T>();
    }

    private static void LoadLibraries()
    {
        var assembly = Assembly.GetExecutingAssembly();

        ReadOnlySpan<string> resourceNames =
            assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(".dll"))
                .ToArray();

        foreach (var resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            _ = AppDomain.CurrentDomain.Load(memoryStream.ToArray());
        }
    }

    internal static void Load()
    {
        LoadLibraries();

        if (HasLoaded)
        {
            Logger.Write("lc-hax has already loaded!");
            return;
        }

        LoadHarmonyPatches();
        LoadHaxModules();
        LoadHaxGameObjects();
    }


    private static void LoadHarmonyPatches()
    {
        try
        {
            new Harmony(HarmonyID).PatchAll();
        }

        catch (Exception exception)
        {
            Logger.Write(exception.ToString());
            throw exception;
        }
    }

    private static void LoadHaxGameObjects()
    {
        DontDestroyOnLoad(HaxGameObjects);

        AddHaxGameObject<HaxObjects>();
        AddHaxGameObject<InputListener>();
        AddHaxGameObject<ScreenListener>();
        AddHaxGameObject<GameListener>();
        AddHaxGameObject<HaxCamera>();
    }

    private static void LoadHaxModules()
    {
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
        AddHaxModules<StunClickMod>();
        AddHaxModules<KillClickMod>();
        AddHaxModules<CrosshairMod>();
        AddHaxModules<MinimalGUIMod>();
        AddHaxModules<PossessionMod>();
        AddHaxModules<DisconnectMod>();
        AddHaxModules<ClearVisionMod>();
        AddHaxModules<InstantInteractMod>();
    }

    internal static void Unload()
    {
        Destroy(HaxModules);
        Destroy(HaxGameObjects);
        new Harmony(HarmonyID).UnpatchAll();
    }
}