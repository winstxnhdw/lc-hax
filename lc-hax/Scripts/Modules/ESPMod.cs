#region

using System;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

sealed class ESPMod : MonoBehaviour {
    internal static ESPMod? Instance { get; private set; }
    RendererPair<PlayerControllerB, SkinnedMeshRenderer>[] PlayerRenderers { get; set; } = [];
    RendererPair<Landmine, Renderer>[] LandmineRenderers { get; set; } = [];
    RendererPair<SpikeRoofTrap, Renderer>[] SpikeRoofTrapRenderers { get; set; } = [];
    RendererPair<Turret, Renderer>[] TurretRenderers { get; set; } = [];

    RendererPair<BreakerBox, Renderer>[] BreakerBoxRenderers { get; set; } = [];

    Renderer[] EntranceRenderers { get; set; } = [];
    Renderer[] StoryLog { get; set; } = [];

    bool InGame { get; set; } = false;
    bool IsMapLoaded { get; set; } = false;
    bool Enabled { get; set; } = true;

    void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void OnEnable() {
        GameListener.OnLevelGenerated += this.Initialise;
        GameListener.OnGameStart += this.Initialise;
        GameListener.OnGameEnd += this.OnGameEnd;
        GameListener.OnLevelGenerated += this.OnMapLoaded;
        GameListener.OnShipLeave += this.OnShipLeave;
        InputListener.OnPausePress += this.ToggleESP;
    }

    void OnDisable() {
        GameListener.OnLevelGenerated -= this.Initialise;
        GameListener.OnGameStart -= this.Initialise;
        GameListener.OnGameEnd -= this.OnGameEnd;
        GameListener.OnLevelGenerated -= this.OnMapLoaded;
        GameListener.OnShipLeave -= this.OnShipLeave;
        InputListener.OnPausePress -= this.ToggleESP;
    }

    void OnGUI() {
        if (!this.Enabled || !this.InGame || Helper.CurrentCamera is not Camera camera) return;

        this.RenderAlways(camera);
        this.RenderWhenMapLoads(camera);
    }

    void RenderAlways(Camera camera) {
        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not PlayerControllerB player) return;
            if (player.IsDead()) return;

            string label = $"#{player.playerClientId} {player.playerUsername}, (Health : {player.health})";

            this.RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                Helper.ExtraColors.Aquamarine, this.RenderLabel(label)
            );
        }, false);

        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject => {
            if (grabbableObject == null) return;
            if (Helper.LocalPlayer is not PlayerControllerB player) return;
            if (grabbableObject.mainObjectRenderer is Renderer renderer && !renderer.enabled) return;
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position);
            if (PossessionMod.Instance is { IsPossessed: true } and not ({ PossessedEnemy: HoarderBugAI } or
                { PossessedEnemy: BaboonBirdAI })) return;
            if (player.HasItemInSlot(grabbableObject)) return;
            if (rendererCentrePoint.z <= 1f) return;

            this.RenderLabel(grabbableObject.BuildGrabbableLabel()).Invoke(
                Helper.GetLootColor(grabbableObject),
                rendererCentrePoint
            );
        }, false);
    }

    void RenderWhenMapLoads(Camera camera) {
        if (!this.IsMapLoaded) return;

        this.BreakerBoxRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not BreakerBox breaker) return;

            if (breaker.isPowerOn)
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.SpringGreen,
                    this.RenderLabel("Breaker Box"));
            else
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.Yellow,
                    this.RenderLabel("Breaker Box (OFF)"));
        }, false);

        this.SpikeRoofTrapRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not SpikeRoofTrap spike) return;

            if (spike.isTrapActive())
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    this.RenderLabel("Spike Roof Trap"));
            else
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    this.RenderLabel("Spike Roof Trap (OFF)"));
        }, false);

        this.LandmineRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    this.RenderLabel("Landmine"));
            else
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    this.RenderLabel("Landmine (OFF)"));
        }, false);
        this.LandmineRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    this.RenderLabel("Landmine"));
            else
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    this.RenderLabel("Landmine (OFF)"));
        }, false);
        this.TurretRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Turret turret) return;

            if (turret.isTurretActive())
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed,
                    this.RenderLabel("Turret"));
            else
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen,
                    this.RenderLabel("Turret (OFF)"));
        }, false);

        this.EntranceRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.LightGoldenrodYellow, this.RenderLabel("Entrance")
        ), false);

        this.StoryLog.Where(x => x.enabled).ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.Violet, this.RenderLabel("Story Log")
        ), false);

        Helper.Enemies.WhereIsNotNull().ForEach(enemy => {
            if (enemy.isEnemyDead) return;
            if (PossessionMod.Instance?.PossessedEnemy == enemy) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees or TestEnemy or ButlerBeesEnemyAI
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (nullableRenderer.Unfake() is not Renderer renderer) return;
            string? EnemyESPLabel = enemy.enemyType.enemyName;

            if (enemy.CanEnemyDie())
                // append health to EnemyESPLabel
                EnemyESPLabel += $" (Health : {enemy.enemyHP})";

            this.RenderBounds(
                camera,
                renderer.bounds,
                Color.red, this.RenderLabel(EnemyESPLabel)
            );
        }, false);

        if (Helper.StartOfRound is { shipBounds: Collider shipBounds })
            this.RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green, this.RenderLabel("Ship"),
                10.0f
            );
    }

    void Initialise() {
        this.InitialiseRenderers();
        this.InitialiseCoordinates();
        this.InGame = true;
        this.IsMapLoaded = Helper.StartOfRound is { inShipPhase: false };
    }

    void OnGameEnd() => this.InGame = false;

    void OnShipLeave() => this.IsMapLoaded = false;

    void OnMapLoaded() => this.IsMapLoaded = true;

    void ToggleESP() => this.Enabled = !this.Enabled;

    Renderer[] GetRenderers<T>() where T : Component =>
        Helper.FindObjects<T>()
            .Select(obj => obj.GetComponent<Renderer>())
            .ToArray();

    Renderer[] GetRenderersInChildren<T>() where T : Component =>
        Helper.FindObjects<T>()
            .Select(obj => obj.GetComponentInChildren<Renderer>())
            .ToArray();

    void InitialiseRenderers() {
        this.PlayerRenderers = Helper.Players.Select(player =>
            new RendererPair<PlayerControllerB, SkinnedMeshRenderer>() {
                GameObject = player,
                Renderer = player.thisPlayerModel
            }
        ).ToArray();

        this.LandmineRenderers = Helper.FindObjects<Landmine>().Select(mine =>
            new RendererPair<Landmine, Renderer>() {
                GameObject = mine,
                Renderer = mine.GetComponent<Renderer>()
            }
        ).ToArray();

        this.SpikeRoofTrapRenderers = Helper.FindObjects<SpikeRoofTrap>().Select(spiketrap =>
            new RendererPair<SpikeRoofTrap, Renderer>() {
                GameObject = spiketrap,
                Renderer = spiketrap.GetComponent<Renderer>()
            }
        ).ToArray();

        this.BreakerBoxRenderers = Helper.FindObjects<BreakerBox>().Where(x => x.name.Contains("(Clone)")).Select(
            breakerbox =>
                new RendererPair<BreakerBox, Renderer>() {
                    GameObject = breakerbox,
                    Renderer = breakerbox.gameObject.FindObject("Mesh")?.GetComponent<Renderer>()
                }
        ).ToArray();

        this.TurretRenderers = Helper.FindObjects<Turret>().Select(turret =>
            new RendererPair<Turret, Renderer>() {
                GameObject = turret,
                Renderer = turret.GetComponent<Renderer>()
            }
        ).ToArray();


        this.EntranceRenderers = this.GetRenderers<EntranceTeleport>();
    }

    void InitialiseCoordinates() => this.StoryLog = this.GetRenderersInChildren<StoryLog>();

    Size GetRendererSize(Bounds bounds, Camera camera) {
        ReadOnlySpan<Vector3> corners = [
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
        Vector2 maxScreenVector = minScreenVector;

        for (int i = 1; i < corners.Length; i++) {
            Vector2 cornerScreen = camera.WorldToEyesPoint(corners[i]);
            minScreenVector = Vector2.Min(minScreenVector, cornerScreen);
            maxScreenVector = Vector2.Max(maxScreenVector, cornerScreen);
        }

        return new Size() {
            Width = Mathf.Abs(maxScreenVector.x - minScreenVector.x),
            Height = Mathf.Abs(maxScreenVector.y - minScreenVector.y)
        };
    }

    void RenderBounds(
        Camera camera,
        Bounds bounds,
        Color colour,
        Action<Color, Vector3>? action,
        float cutOffDistance = 1f
    ) {
        Vector3 rendererCentrePoint = camera.WorldToEyesPoint(bounds.center);

        if (rendererCentrePoint.z <= cutOffDistance) return;

        Helper.DrawOutlineBox(
            rendererCentrePoint, this.GetRendererSize(bounds, camera),
            1.0f,
            colour
        );

        action?.Invoke(colour, rendererCentrePoint);
    }

    void RenderBounds(Camera camera, Bounds bounds, Action<Color, Vector3>? action) =>
        this.RenderBounds(camera, bounds, Color.white, action);

    Action<Color, Vector3> RenderLabel(string name) =>
        (colour, rendererCentrePoint) =>
            Helper.DrawLabel(rendererCentrePoint, name, colour);
}
