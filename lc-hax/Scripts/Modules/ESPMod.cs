using System;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

sealed class ESPMod : MonoBehaviour {
    RendererPair<PlayerControllerB, SkinnedMeshRenderer>[] PlayerRenderers { get; set; } = [];
    Renderer[] LandmineRenderers { get; set; } = [];
    Renderer[] TurretRenderers { get; set; } = [];
    Renderer[] EntranceRenderers { get; set; } = [];
    Vector3[] StoryLogVectors { get; set; } = [];

    bool InGame { get; set; }
    bool IsMapLoaded { get; set; }
    bool Enabled { get; set; } = true;

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
        if (!this.Enabled) return;
        if (!this.InGame) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        this.RenderAlways(camera);
        this.RenderWhenMapLoads(camera);
    }

    void RenderAlways(Camera camera) {
        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not PlayerControllerB player) return;
            if (player.isPlayerDead) return;
            if (!player.isPlayerControlled) return;

            string label = $"#{player.playerClientId} {player.playerUsername}";

            RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                ESPMod.RenderLabel(label)
            );
        });

        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject => {
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position);

            if (rendererCentrePoint.z <= 2.0f) {
                return;
            }

            ESPMod.RenderLabel($"{grabbableObject.itemProperties.itemName} ${grabbableObject.scrapValue}").Invoke(
                Color.gray,
                rendererCentrePoint
            );
        });
    }

    void RenderWhenMapLoads(Camera camera) {
        if (!this.IsMapLoaded) return;

        this.LandmineRenderers.ForEach(renderer => RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            ESPMod.RenderLabel("Landmine")
        ));

        this.TurretRenderers.ForEach(renderer => RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            ESPMod.RenderLabel("Turret")
        ));

        this.EntranceRenderers.ForEach(renderer => RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            ESPMod.RenderLabel("Entrance")
        ));

        this.StoryLogVectors.ForEach(vector => {
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(vector);

            if (rendererCentrePoint.z <= 2.0f) {
                return;
            }

            ESPMod.RenderLabel("Log").Invoke(Color.gray, rendererCentrePoint);
        });

        Helper.Enemies.WhereIsNotNull().ForEach(enemy => {
            if (enemy.isEnemyDead) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (nullableRenderer.Unfake() is not Renderer renderer) {
                return;
            }

            RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                ESPMod.RenderLabel(enemy.enemyType.enemyName)
            );
        });

        if (Helper.StartOfRound is { shipBounds: Collider shipBounds }) {
            RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                ESPMod.RenderLabel("Ship"),
                10.0f
            );
        }
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

    static Renderer[] GetRenderers<T>() where T : Component =>
        Helper.FindObjects<T>()
              .AsValueEnumerable()
              .Select(obj => obj.GetComponent<Renderer>())
              .ToArray();

    void InitialiseRenderers() {
        this.PlayerRenderers = Helper.Players.AsValueEnumerable().Select(player => new RendererPair<PlayerControllerB, SkinnedMeshRenderer>() {
            GameObject = player,
            Renderer = player.thisPlayerModel
        }).ToArray();

        this.LandmineRenderers = ESPMod.GetRenderers<Landmine>();
        this.TurretRenderers = ESPMod.GetRenderers<Turret>();
        this.EntranceRenderers = ESPMod.GetRenderers<EntranceTeleport>();
    }

    void InitialiseCoordinates() =>
        this.StoryLogVectors =
            Helper.FindObjects<StoryLog>()
                  .AsValueEnumerable()
                  .Select(log => log.transform.position)
                  .ToArray();

    static Size GetRendererSize(Bounds bounds, Camera camera) {
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

    static void RenderBounds(
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
            ESPMod.GetRendererSize(bounds, camera),
            1.0f,
            colour
        );

        action?.Invoke(colour, rendererCentrePoint);
    }

    static void RenderBounds(Camera camera, Bounds bounds, Action<Color, Vector3>? action) =>
        ESPMod.RenderBounds(camera, bounds, Color.white, action);

    static Action<Color, Vector3> RenderLabel(string name) => (colour, rendererCentrePoint) =>
        Helper.DrawLabel(rendererCentrePoint, name, colour);
}
