using GameNetcodeStuff;
using Hax;
using System;
using System.Linq;
using UnityEngine;

internal sealed class ESPMod : MonoBehaviour {
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

    private void OnEnable() {
        GameListener.OnLevelGenerated += this.Initialise;
        GameListener.OnGameStart += this.Initialise;
        GameListener.OnGameEnd += this.OnGameEnd;
        GameListener.OnLevelGenerated += this.OnMapLoaded;
        GameListener.OnShipLeave += this.OnShipLeave;
        InputListener.OnPausePress += this.ToggleESP;
    }

    private void OnDisable() {
        GameListener.OnLevelGenerated -= this.Initialise;
        GameListener.OnGameStart -= this.Initialise;
        GameListener.OnGameEnd -= this.OnGameEnd;
        GameListener.OnLevelGenerated -= this.OnMapLoaded;
        GameListener.OnShipLeave -= this.OnShipLeave;
        InputListener.OnPausePress -= this.ToggleESP;
    }

    private void OnGUI() {
        if (!this.Enabled || !this.InGame || Helper.CurrentCamera is not Camera camera) return;

        this.RenderAlways(camera);
        this.RenderWhenMapLoads(camera);
    }

    private void RenderAlways(Camera camera) {
        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not PlayerControllerB player) return;
            if (player.isPlayerDead || !player.isPlayerControlled) return;

            string label = $"#{player.playerClientId} {player.playerUsername}, (Health : {player.health})";

            this.RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                Helper.ExtraColors.Aquamarine,
                this.RenderLabel(label)
            );
        });

        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject => {
            if (grabbableObject == null) return;
            if(!grabbableObject.enabled) return;
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position);

            if (PossessionMod.Instance is { IsPossessed: true } and not ({ PossessedEnemy: HoarderBugAI } or { PossessedEnemy: BaboonBirdAI })) return;

            if (rendererCentrePoint.z <= 2.0f) {
                return;
            }

            this.RenderLabel($"{grabbableObject.ToEspLabel()} ${grabbableObject.GetScrapValue()}").Invoke(
                Helper.GetLootColor(grabbableObject),
                rendererCentrePoint
            );
        });
    }

    private void RenderWhenMapLoads(Camera camera) {
        if (!this.IsMapLoaded) return;

        this.BreakerBoxRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not BreakerBox breaker) return;

            if (breaker.isPowerOn) {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.SpringGreen, this.RenderLabel("Breaker Box (ON)"));
            }
            else {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.Yellow, this.RenderLabel("Breaker Box (OFF)"));
            }
        });

        this.SpikeRoofTrapRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not SpikeRoofTrap spike) return;

            if (spike.isTrapActive()) {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed, this.RenderLabel("Spike Roof Trap"));
            }
            else {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen, this.RenderLabel("Spike Roof Trap (OFF)"));
            }
        });

        this.LandmineRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed, this.RenderLabel("Landmine"));
            }
            else
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen, this.RenderLabel("Landmine (OFF)"));
            }
        });
        this.LandmineRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Landmine mine) return;
            if (mine.hasExploded) return;

            if (mine.isLandmineActive())
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed, this.RenderLabel("Landmine"));
            }
            else
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen, this.RenderLabel("Landmine (OFF)"));
            }
        });
        this.TurretRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not Turret turret) return;

            if (turret.isTurretActive())
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.OrangeRed, this.RenderLabel("Turret"));
            }
            else
            {
                this.RenderBounds(camera, rendererPair.Renderer.bounds, Helper.ExtraColors.YellowGreen, this.RenderLabel("Turret (OFF)"));
            }
        });

        this.EntranceRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.LightGoldenrodYellow,
            this.RenderLabel("Entrance")
        ));

        this.StoryLog.Where(x => x.enabled).ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Helper.ExtraColors.Violet,
            this.RenderLabel("Story Log")
        ));

        Helper.Enemies.WhereIsNotNull().ForEach(enemy => {
            if (enemy.isEnemyDead) return;
            if (PossessionMod.Instance?.PossessedEnemy == enemy) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees or TestEnemy or ButlerBeesEnemyAI
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (nullableRenderer.Unfake() is not Renderer renderer) {
                return;
            }
            string EnemyESPLabel = enemy.enemyType.enemyName;

            if(enemy.CanEnemyDie())
            {
                // append health to EnemyESPLabel
                EnemyESPLabel += $" (Health : {enemy.enemyHP})";
            }

            this.RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                this.RenderLabel(EnemyESPLabel)   
            );
        });

        if (Helper.StartOfRound is { shipBounds: Collider shipBounds }) {
            this.RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                this.RenderLabel("Ship"),
                10.0f
            );
        }
    }

    private void Initialise() {
        this.InitialiseRenderers();
        this.InitialiseCoordinates();
        this.InGame = true;
        this.IsMapLoaded = Helper.StartOfRound is { inShipPhase: false };
    }

    private void OnGameEnd() => this.InGame = false;

    private void OnShipLeave() => this.IsMapLoaded = false;

    private void OnMapLoaded() => this.IsMapLoaded = true;

    private void ToggleESP() => this.Enabled = !this.Enabled;

    private Renderer[] GetRenderers<T>() where T : Component =>
        Helper.FindObjects<T>()
              .Select(obj => obj.GetComponent<Renderer>())
              .ToArray();

    private Renderer[] GetRenderersInChildren<T>() where T : Component =>
        Helper.FindObjects<T>()
            .Select(obj => obj.GetComponentInChildren<Renderer>())
            .ToArray();

    private void InitialiseRenderers() {
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

        this.BreakerBoxRenderers = Helper.FindObjects<BreakerBox>().Where(x => x.name.Contains("(Clone)")).Select(breakerbox =>
            new RendererPair<BreakerBox, Renderer>() {
                GameObject = breakerbox,
                Renderer = breakerbox.gameObject.FindObject("Mesh")?.GetComponent<Renderer>()
            }
            ).ToArray();

        this.TurretRenderers = Helper.FindObjects<Turret>().Select(turret =>
            new RendererPair<Turret, Renderer>()
            {
                GameObject = turret,
                Renderer = turret.GetComponent<Renderer>()
            }
            ).ToArray();


        this.EntranceRenderers = this.GetRenderers<EntranceTeleport>();
    }

    private void InitialiseCoordinates() => this.StoryLog = this.GetRenderersInChildren<StoryLog>();

    private Size GetRendererSize(Bounds bounds, Camera camera) {
        ReadOnlySpan<Vector3> corners = [
            new(bounds.min.x, bounds.min.y, bounds.min.z),
            new(bounds.max.x, bounds.min.y, bounds.min.z),
            new(bounds.min.x, bounds.max.y, bounds.min.z),
            new(bounds.max.x, bounds.max.y, bounds.min.z),
            new(bounds.min.x, bounds.min.y, bounds.max.z),
            new(bounds.max.x, bounds.min.y, bounds.max.z),
            new(bounds.min.x, bounds.max.y, bounds.max.z),
            new(bounds.max.x, bounds.max.y, bounds.max.z)
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

    private void RenderBounds(
        Camera camera,
        Bounds bounds,
        Color colour,
        Action<Color, Vector3>? action,
        float cutOffDistance = 4.0f
    ) {
        Vector3 rendererCentrePoint = camera.WorldToEyesPoint(bounds.center);

        if (rendererCentrePoint.z <= cutOffDistance) {
            return;
        }

        Helper.DrawOutlineBox(
            rendererCentrePoint,
            this.GetRendererSize(bounds, camera),
            1.0f,
            colour
        );

        action?.Invoke(colour, rendererCentrePoint);
    }

    private void RenderBounds(Camera camera, Bounds bounds, Action<Color, Vector3>? action) =>
        this.RenderBounds(camera, bounds, Color.white, action);

    private Action<Color, Vector3> RenderLabel(string name) => (colour, rendererCentrePoint) =>
        Helper.DrawLabel(rendererCentrePoint, name, colour);
}
