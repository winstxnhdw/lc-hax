using System;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class ESPMod : MonoBehaviour
{
    internal static ESPMod? Instance { get; private set; }
    private RendererPair<PlayerControllerB, SkinnedMeshRenderer>[] PlayerRenderers { get; set; } = [];
    private RendererPair<Landmine, Renderer>[] LandmineRenderers { get; set; } = [];
    private RendererPair<SpikeRoofTrap, Renderer>[] SpikeRoofTrapRenderers { get; set; } = [];
    private RendererPair<Turret, Renderer>[] TurretRenderers { get; set; } = [];

    private RendererPair<BreakerBox, Renderer>[] BreakerBoxRenderers { get; set; } = [];

    private Renderer[] EntranceRenderers { get; set; } = [];
    private Renderer[] StoryLog { get; set; } = [];

    private bool InGame { get; set; } = false;
    private bool IsMapLoaded { get; set; } = false;
    private bool Enabled { get; set; } = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameListener.OnLevelGenerated += Initialise;
        GameListener.OnGameStart += Initialise;
        GameListener.OnGameEnd += OnGameEnd;
        GameListener.OnLevelGenerated += OnMapLoaded;
        GameListener.OnShipLeave += OnShipLeave;
        InputListener.OnPausePress += ToggleESP;
    }

    private void OnDisable()
    {
        GameListener.OnLevelGenerated -= Initialise;
        GameListener.OnGameStart -= Initialise;
        GameListener.OnGameEnd -= OnGameEnd;
        GameListener.OnLevelGenerated -= OnMapLoaded;
        GameListener.OnShipLeave -= OnShipLeave;
        InputListener.OnPausePress -= ToggleESP;
    }

    private void OnGUI()
    {
        if (!Enabled || !InGame || Helper.CurrentCamera is not Camera camera) return;

        RenderAlways(camera);
        RenderWhenMapLoads(camera);
    }

    private void RenderAlways(Camera camera)
    {
        PlayerRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not PlayerControllerB player) return;
            if (player.IsDead()) return;

            var label = $"#{player.playerClientId} {player.playerUsername}, (Health : {player.health})";

            RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                Helper.ExtraColors.Aquamarine,
                RenderLabel(label)
            );
        }, false);

        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject =>
        {
            if (grabbableObject == null) return;
            if (Helper.LocalPlayer is not PlayerControllerB player) return;
            if (grabbableObject.mainObjectRenderer is Renderer renderer && !renderer.enabled) return;
            var rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position);
            if (PossessionMod.Instance is { IsPossessed: true } and not ({ PossessedEnemy: HoarderBugAI } or
                { PossessedEnemy: BaboonBirdAI })) return;
            if (player.HasItemInSlot(grabbableObject)) return;
            if (rendererCentrePoint.z <= 1f) return;

            RenderLabel(grabbableObject.BuildGrabbableLabel()).Invoke(
                Helper.GetLootColor(grabbableObject),
                rendererCentrePoint
            );
        }, false);
    }

    private void RenderWhenMapLoads(Camera camera)
    {
        if (!IsMapLoaded) return;

        BreakerBoxRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not BreakerBox breaker) return;

            if (breaker.isPowerOn)
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.SpringGreen,
                    RenderLabel("Breaker Box"));
            else
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.Yellow,
                    RenderLabel("Breaker Box (OFF)"));
        }, false);

        SpikeRoofTrapRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not SpikeRoofTrap spike) return;

            if (spike.isTrapActive())
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    RenderLabel("Spike Roof Trap"));
            else
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    RenderLabel("Spike Roof Trap (OFF)"));
        }, false);

        LandmineRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    RenderLabel("Landmine"));
            else
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    RenderLabel("Landmine (OFF)"));
        }, false);
        LandmineRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    RenderLabel("Landmine"));
            else
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    RenderLabel("Landmine (OFF)"));
        }, false);
        TurretRenderers.ForEach(rendererPair =>
        {
            if (rendererPair.GameObject is not Turret turret) return;

            if (turret.isTurretActive())
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed, RenderLabel("Turret"));
            else
                RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    RenderLabel("Turret (OFF)"));
        }, false);

        EntranceRenderers.ForEach(renderer => RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.LightGoldenrodYellow,
            RenderLabel("Entrance")
        ), false);

        StoryLog.Where(x => x.enabled).ForEach(renderer => RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.Violet,
            RenderLabel("Story Log")
        ), false);

        Helper.Enemies.WhereIsNotNull().ForEach(enemy =>
        {
            if (enemy.isEnemyDead) return;
            if (PossessionMod.Instance?.PossessedEnemy == enemy) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees or TestEnemy or ButlerBeesEnemyAI
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (nullableRenderer.Unfake() is not Renderer renderer) return;
            var EnemyESPLabel = enemy.enemyType.enemyName;

            if (enemy.CanEnemyDie())
                // append health to EnemyESPLabel
                EnemyESPLabel += $" (Health : {enemy.enemyHP})";

            RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                RenderLabel(EnemyESPLabel)
            );
        }, false);

        if (Helper.StartOfRound is { shipBounds: Collider shipBounds })
            RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                RenderLabel("Ship"),
                10.0f
            );
    }

    private void Initialise()
    {
        InitialiseRenderers();
        InitialiseCoordinates();
        InGame = true;
        IsMapLoaded = Helper.StartOfRound is { inShipPhase: false };
    }

    private void OnGameEnd()
    {
        InGame = false;
    }

    private void OnShipLeave()
    {
        IsMapLoaded = false;
    }

    private void OnMapLoaded()
    {
        IsMapLoaded = true;
    }

    private void ToggleESP()
    {
        Enabled = !Enabled;
    }

    private Renderer[] GetRenderers<T>() where T : Component
    {
        return Helper.FindObjects<T>()
            .Select(obj => obj.GetComponent<Renderer>())
            .ToArray();
    }

    private Renderer[] GetRenderersInChildren<T>() where T : Component
    {
        return Helper.FindObjects<T>()
            .Select(obj => obj.GetComponentInChildren<Renderer>())
            .ToArray();
    }

    private void InitialiseRenderers()
    {
        PlayerRenderers = Helper.Players.Select(player =>
            new RendererPair<PlayerControllerB, SkinnedMeshRenderer>()
            {
                GameObject = player,
                Renderer = player.thisPlayerModel
            }
        ).ToArray();

        LandmineRenderers = Helper.FindObjects<Landmine>().Select(mine =>
            new RendererPair<Landmine, Renderer>()
            {
                GameObject = mine,
                Renderer = mine.GetComponent<Renderer>()
            }
        ).ToArray();

        SpikeRoofTrapRenderers = Helper.FindObjects<SpikeRoofTrap>().Select(spiketrap =>
            new RendererPair<SpikeRoofTrap, Renderer>()
            {
                GameObject = spiketrap,
                Renderer = spiketrap.GetComponent<Renderer>()
            }
        ).ToArray();

        BreakerBoxRenderers = Helper.FindObjects<BreakerBox>().Where(x => x.name.Contains("(Clone)")).Select(
            breakerbox =>
                new RendererPair<BreakerBox, Renderer>()
                {
                    GameObject = breakerbox,
                    Renderer = breakerbox.gameObject.FindObject("Mesh")?.GetComponent<Renderer>()
                }
        ).ToArray();

        TurretRenderers = Helper.FindObjects<Turret>().Select(turret =>
            new RendererPair<Turret, Renderer>()
            {
                GameObject = turret,
                Renderer = turret.GetComponent<Renderer>()
            }
        ).ToArray();


        EntranceRenderers = GetRenderers<EntranceTeleport>();
    }

    private void InitialiseCoordinates()
    {
        StoryLog = GetRenderersInChildren<StoryLog>();
    }

    private Size GetRendererSize(Bounds bounds, Camera camera)
    {
        ReadOnlySpan<Vector3> corners =
        [
            new Vector3(bounds.min.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.max.z)
        ];

        Vector2 minScreenVector = camera.WorldToEyesPoint(corners[0]);
        var maxScreenVector = minScreenVector;

        for (var i = 1; i < corners.Length; i++)
        {
            Vector2 cornerScreen = camera.WorldToEyesPoint(corners[i]);
            minScreenVector = Vector2.Min(minScreenVector, cornerScreen);
            maxScreenVector = Vector2.Max(maxScreenVector, cornerScreen);
        }

        return new Size()
        {
            Width = Mathf.Abs(maxScreenVector.x - minScreenVector.x),
            Height = Mathf.Abs(maxScreenVector.y - minScreenVector.y)
        };
    }

    private void RenderBounds(
        Camera camera,
        Bounds bounds,
        Color colour,
        Action<Color, Vector3>? action,
        float cutOffDistance = 1f
    )
    {
        var rendererCentrePoint = camera.WorldToEyesPoint(bounds.center);

        if (rendererCentrePoint.z <= cutOffDistance) return;

        Helper.DrawOutlineBox(
            rendererCentrePoint,
            GetRendererSize(bounds, camera),
            1.0f,
            colour
        );

        action?.Invoke(colour, rendererCentrePoint);
    }

    private void RenderBounds(Camera camera, Bounds bounds, Action<Color, Vector3>? action)
    {
        RenderBounds(camera, bounds, Color.white, action);
    }

    private Action<Color, Vector3> RenderLabel(string name)
    {
        return (colour, rendererCentrePoint) =>
            Helper.DrawLabel(rendererCentrePoint, name, colour);
    }
}