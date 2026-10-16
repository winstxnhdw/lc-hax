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
    Plane[] FrustumPlanes { get; } = new Plane[6];
    Plane[] SidePlanes { get; } = new Plane[4];
    CachedLabel DynamicLabel { get; } = new();
    CachedLabel LandmineLabel { get; } = new() { Text = "Landmine" };
    CachedLabel TurretLabel { get; } = new() { Text = "Turret" };
    CachedLabel EntranceLabel { get; } = new() { Text = "Entrance" };
    CachedLabel StoryLogLabel { get; } = new() { Text = "Log" };
    CachedLabel ShipLabel { get; } = new() { Text = "Ship" };
    Rect ScreenBounds { get; set; }

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
        if (Event.current.type != EventType.Repaint) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        this.ScreenBounds = new Rect(0, 0, Screen.width, Screen.height);
        _ = Helper.BeginLabelRepaint();
        GeometryUtility.CalculateFrustumPlanes(camera, this.FrustumPlanes);
        Array.Copy(this.FrustumPlanes, this.SidePlanes, this.SidePlanes.Length);
        this.RenderAlways(camera);
        this.RenderWhenMapLoads(camera);
    }

    void RenderAlways(Camera camera) {
        foreach (RendererPair<PlayerControllerB, SkinnedMeshRenderer> rendererPair in this.PlayerRenderers) {
            if (rendererPair.GameObject is not PlayerControllerB player) continue;
            if (player.isPlayerDead) continue;
            if (!player.isPlayerControlled) continue;

            this.DynamicLabel.Text = $"#{player.playerClientId} {player.playerUsername}";
            this.RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                this.DynamicLabel
            );
        }

        foreach (GrabbableObject grabbableObject in Helper.Grabbables) {
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position, this.ScreenBounds);

            if (rendererCentrePoint.z <= 2.0f) {
                continue;
            }

            this.DynamicLabel.Text = $"{grabbableObject.itemProperties.itemName} ${grabbableObject.scrapValue}";
            Helper.DrawLabel(
                rendererCentrePoint,
                this.DynamicLabel,
                Color.gray,
                this.ScreenBounds
            );
        }
    }

    void RenderWhenMapLoads(Camera camera) {
        if (!this.IsMapLoaded) return;

        foreach (Renderer renderer in this.LandmineRenderers) {
            this.RenderBounds(camera, renderer.bounds, Color.yellow, this.LandmineLabel);
        }

        foreach (Renderer renderer in this.TurretRenderers) {
            this.RenderBounds(camera, renderer.bounds, Color.yellow, this.TurretLabel);
        }

        foreach (Renderer renderer in this.EntranceRenderers) {
            this.RenderBounds(camera, renderer.bounds, Color.yellow, this.EntranceLabel);
        }

        foreach (Vector3 vector in this.StoryLogVectors) {
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(vector, this.ScreenBounds);

            if (rendererCentrePoint.z <= 2.0f) {
                continue;
            }

            Helper.DrawLabel(rendererCentrePoint, this.StoryLogLabel, Color.gray, this.ScreenBounds);
        }

        foreach (EnemyAI enemy in Helper.Enemies) {
            if (enemy is null) continue;
            if (enemy.isEnemyDead) continue;
            if (enemy is DocileLocustBeesAI or DoublewingAI) continue;

            Renderer? nullableRenderer = enemy is RedLocustBees
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (nullableRenderer.Unfake() is not Renderer renderer) {
                continue;
            }

            this.DynamicLabel.Text = enemy.enemyType.enemyName;
            this.RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                this.DynamicLabel
            );
        }

        if (Helper.StartOfRound is { shipBounds: Collider shipBounds }) {
            this.RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                this.ShipLabel,
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

    static Size GetRendererSize(Bounds bounds, Camera camera, Rect screenBounds) {
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

        Vector2 minScreenVector = camera.WorldToEyesPoint(corners[0], screenBounds);
        Vector2 maxScreenVector = minScreenVector;

        for (int i = 1; i < corners.Length; i++) {
            Vector2 cornerScreen = camera.WorldToEyesPoint(corners[i], screenBounds);
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
        CachedLabel label,
        float cutOffDistance = 4.0f
    ) {
        Vector3 rendererCentrePoint = camera.WorldToEyesPoint(bounds.center, this.ScreenBounds);

        if (rendererCentrePoint.z <= cutOffDistance) {
            return;
        }

        if (!GeometryUtility.TestPlanesAABB(this.SidePlanes, bounds)) {
            return;
        }

        Helper.DrawOutlineBox(
            rendererCentrePoint,
            ESPMod.GetRendererSize(bounds, camera, this.ScreenBounds),
            1.0f,
            colour
        );

        Helper.DrawLabel(rendererCentrePoint, label, colour, this.ScreenBounds);
    }

    void RenderBounds(Camera camera, Bounds bounds, CachedLabel label) =>
        this.RenderBounds(camera, bounds, Color.white, label);
}
