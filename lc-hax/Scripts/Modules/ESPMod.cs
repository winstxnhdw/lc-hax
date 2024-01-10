using System;
using System.Linq;
using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class ESPMod : MonoBehaviour {
    IEnumerable<RendererPair<PlayerControllerB, SkinnedMeshRenderer>> PlayerRenderers { get; set; } = [];
    IEnumerable<Renderer> LandmineRenderers { get; set; } = [];
    IEnumerable<Renderer> TurretRenderers { get; set; } = [];
    IEnumerable<Renderer> EntranceRenderers { get; set; } = [];

    bool InGame { get; set; } = false;

    void OnEnable() {
        GameListener.onGameStart += this.OnGameJoin;
        GameListener.onGameEnd += this.OnGameEnd;
        GameListener.onShipLand += this.InitialiseRenderers;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.OnGameJoin;
        GameListener.onGameEnd -= this.OnGameEnd;
        GameListener.onShipLand -= this.InitialiseRenderers;
    }

    void OnGUI() {
        if (!this.InGame || !Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject.isPlayerDead || !rendererPair.GameObject.isPlayerControlled) return;

            PlayerControllerB player = rendererPair.GameObject;
            string label = $"#{player.playerClientId} {player.playerUsername}";

            this.RenderBounds(
                camera,
                rendererPair.Renderer.bounds,
                this.RenderLabel(label)
            );
        });

        this.LandmineRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            this.RenderLabel("Landmine")
        ));

        this.TurretRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            this.RenderLabel("Turret")
        ));

        this.EntranceRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer.bounds,
            Color.yellow,
            this.RenderLabel("Entrance")
        ));

        HaxObjects.Instance?.EnemyAIs.ForEach(nullableEnemy => {
            if (!nullableEnemy.IsNotNull(out EnemyAI enemy)) return;
            if (enemy.isEnemyDead) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            Renderer? nullableRenderer = enemy is RedLocustBees
                ? enemy.meshRenderers.First()
                : enemy.skinnedMeshRenderers.First();

            if (!nullableRenderer.IsNotNull(out Renderer renderer)) {
                return;
            }

            this.RenderBounds(
                camera,
                renderer.bounds,
                Color.red,
                this.RenderLabel(enemy.enemyType.enemyName)
            );
        });

        HaxObjects.Instance?.GrabbableObjects.Objects.ForEach(nullableGrabbableObject => {
            if (!nullableGrabbableObject.IsNotNull(out GrabbableObject grabbableObject)) return;

            Renderer? nullableRenderer = grabbableObject is LungProp lungProp
                ? lungProp.lungDeviceMesh
                : grabbableObject.mainObjectRenderer;

            if (!nullableRenderer.IsNotNull(out Renderer renderer)) {
                return;
            }

            Vector3 rendererCentrePoint = camera.WorldToEyesPoint(renderer.bounds.center);

            if (rendererCentrePoint.z <= 2.0f) {
                return;
            }

            this.RenderLabel($"{grabbableObject.itemProperties.itemName} ${grabbableObject.scrapValue}").Invoke(
                Color.gray,
                rendererCentrePoint
            );
        });

        if (Helper.StartOfRound?.shipBounds.IsNotNull(out Collider shipBounds) is true) {
            this.RenderBounds(
                camera,
                shipBounds.bounds,
                Color.green,
                this.RenderLabel("Ship")
            );
        }
    }

    void OnGameJoin() {
        this.InitialiseRenderers();
        this.InGame = true;
    }

    void OnGameEnd() => this.InGame = false;

    IEnumerable<Renderer> GetRenderers<T>() where T : Component =>
        Helper.FindObjects<T>()
              .Where(obj => obj != null)
              .Select(obj => obj.GetComponent<Renderer>())
              .Where(renderer => renderer != null);

    void InitialiseRenderers() {
        this.PlayerRenderers = Helper.Players.Select(player =>
            new RendererPair<PlayerControllerB, SkinnedMeshRenderer>(player, player.thisPlayerModel)
        );

        this.LandmineRenderers = this.GetRenderers<Landmine>();
        this.TurretRenderers = this.GetRenderers<Turret>();
        this.EntranceRenderers = this.GetRenderers<EntranceTeleport>();
    }

    Size GetRendererSize(Bounds bounds, Camera camera) {
        Vector3[] corners = [
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

    void RenderBounds(Camera camera, Bounds bounds, Color colour, Action<Color, Vector3>? action) {
        Vector3 rendererCentrePoint = camera.WorldToEyesPoint(bounds.center);

        if (rendererCentrePoint.z <= 4.0f) {
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
