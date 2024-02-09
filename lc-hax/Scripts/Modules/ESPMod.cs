using System;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;
using Hax;

internal class ESPMod : MonoBehaviour {
    RendererPair<PlayerControllerB, SkinnedMeshRenderer>[] PlayerRenderers { get; set; } = [];
    Renderer[] LandmineRenderers { get; set; } = [];
    Renderer[] TurretRenderers { get; set; } = [];
    Renderer[] EntranceRenderers { get; set; } = [];
    Vector3[] StoryLogVectors { get; set; } = [];

    bool InGame { get; set; } = false;
    bool Enabled { get; set; } = true;

    void OnEnable() {
        GameListener.onGameStart += this.OnGameJoin;
        GameListener.onGameEnd += this.OnGameEnd;
        GameListener.onShipLand += this.InitialiseRenderers;
        InputListener.OnPausePress += this.ToggleESP;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.OnGameJoin;
        GameListener.onGameEnd -= this.OnGameEnd;
        GameListener.onShipLand -= this.InitialiseRenderers;
        InputListener.OnPausePress -= this.ToggleESP;
    }

    void OnGUI() {
        if (!this.Enabled || !this.InGame || Helper.CurrentCamera is not Camera camera) return;

        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject is not PlayerControllerB player) return;
            if (player.isPlayerDead || !player.isPlayerControlled) return;
            string label = $"#{player.playerClientId} {player.playerUsername}";
            this.RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                Helper.ExtraColors.LimeGreen,
                this.RenderLabel(label)
            );
        });

        this.LandmineRenderers.WhereIsNotNull().ForEach(renderer => {
            if (renderer == null) return;
            this.RenderBounds(
                camera,
                renderer.bounds,
                Helper.ExtraColors.OrangeRed,
                this.RenderLabel("Landmine")
            );
        });

        this.TurretRenderers.WhereIsNotNull().ForEach(renderer => {
            if (renderer == null) return;
            this.RenderBounds(
                camera,
                renderer.bounds,
                Helper.ExtraColors.OrangeRed,
                this.RenderLabel("Turret")
            );
        });

        this.EntranceRenderers.WhereIsNotNull().ForEach(renderer => {
            if (renderer == null) return;
            this.RenderBounds(
                camera,
                renderer.bounds,
                Helper.ExtraColors.LightGreen,
                this.RenderLabel("Entrance")
            );
        });

        this.StoryLogVectors.ForEach(vector => {
            if (vector.z <= 2.0f) return;
            this.RenderLabel("Log").Invoke(Helper.ExtraColors.Silver, vector);
        });

        this.StoryLogVectors.ForEach(vector => {
            if (vector.z <= 2.0f) return;
            this.RenderLabel("Log").Invoke(Color.gray, vector);
        });

        Helper.Enemies.WhereIsNotNull().ForEach(enemy => {
            if (enemy == null) return;
            if (enemy.isEnemyDead) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();
            if (nullableRenderer == null) return;
            if (nullableRenderer.Unfake() is not Renderer renderer) {
                return;
            }

            this.RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                this.RenderLabel(enemy.enemyType.enemyName)
            );
        });

        Helper.Grabbables.WhereIsNotNull().ForEach(grabbableObject => {
            if (grabbableObject == null) return;
            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(grabbableObject.transform.position);

            if (rendererCentrePoint.z <= 2.0f) {
                return;
            }

            string name = grabbableObject.itemProperties.itemName;
            if (grabbableObject is RagdollGrabbableObject ragdollGrabbableObject) {
                PlayerControllerB? player = ragdollGrabbableObject.GetPlayerFromBody();
                name = player == null ? name : $"Body of {player.playerUsername}";
            }
            this.RenderLabel($"{name} ${grabbableObject.scrapValue}").Invoke(
                Helper.GetLootColor(grabbableObject),
                rendererCentrePoint
            );
        });

        if (Helper.StartOfRound?.shipBounds is Collider shipBounds) {
            this.RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                this.RenderLabel("Ship"),
                10.0f
            );
        }
    }

    void OnGameJoin() {
        this.InitialiseRenderers();
        this.InGame = true;
    }

    void OnGameEnd() => this.InGame = false;

    void ToggleESP() => this.Enabled = !this.Enabled;

    Renderer[] GetRenderers<T>() where T : Component =>
        Helper.FindObjects<T>()
              .Select(obj => obj.GetComponent<Renderer>())
              .ToArray();

    void InitialiseRenderers() {
        this.PlayerRenderers = Helper.Players.Select(player =>
            new RendererPair<PlayerControllerB, SkinnedMeshRenderer>(player, player.thisPlayerModel)
        ).ToArray();

        this.LandmineRenderers = this.GetRenderers<Landmine>();
        this.TurretRenderers = this.GetRenderers<Turret>();
        this.EntranceRenderers = this.GetRenderers<EntranceTeleport>();
        this.InitialiseCoordinates();
    }

    void InitialiseCoordinates() => this.StoryLogVectors = Helper.FindObjects<StoryLog>().Select(log => log.transform.position).ToArray();

    Size GetRendererSize(Bounds bounds, Camera camera) {
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

        return new Size(
            Mathf.Abs(maxScreenVector.x - minScreenVector.x),
            Mathf.Abs(maxScreenVector.y - minScreenVector.y)
        );
    }

    void RenderBounds(
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

    void RenderBounds(Camera camera, Bounds bounds, Action<Color, Vector3>? action) =>
        this.RenderBounds(camera, bounds, Color.white, action);

    Action<Color, Vector3> RenderLabel(string name) => (colour, rendererCentrePoint) =>
        Helper.DrawLabel(rendererCentrePoint, name, colour);
}
