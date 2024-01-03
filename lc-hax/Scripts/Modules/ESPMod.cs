using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Hax;

public class ESPMod : MonoBehaviour {
    IEnumerable<RendererPair<PlayerControllerB>> PlayerRenderers { get; set; } = [];
    IEnumerable<Renderer> LandmineRenderers { get; set; } = [];
    IEnumerable<Renderer> TurretRenderers { get; set; } = [];

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

    void Start() {
        this.InitialiseRenderers();
    }

    void OnGUI() {
        if (!this.InGame || !Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        this.PlayerRenderers.ForEach(rendererPair => {
            if (rendererPair.GameObject.isPlayerDead) return;

            this.RenderBounds(
                camera,
                rendererPair.Renderer,
                this.RenderPlayer(rendererPair.GameObject)
            );
        });

        this.LandmineRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer,
            Color.yellow,
            this.RenderObject("Landmine")
        ));

        this.TurretRenderers.ForEach(renderer => this.RenderBounds(
            camera,
            renderer,
            Color.yellow,
            this.RenderObject("Turret")
        ));

        HaxObjects.Instance?.EnemyAIs.ForEach(nullableEnemy => {
            if (!nullableEnemy.IsNotNull(out EnemyAI enemy)) return;
            if (enemy is DocileLocustBeesAI or DoublewingAI) return;

            this.RenderBounds(
                camera,
                enemy.skinnedMeshRenderers[0],
                Color.red,
                this.RenderEnemy(enemy)
            );
        });
    }

    void OnGameJoin() {
        this.InitialiseRenderers();
        this.InGame = true;
    }

    void OnGameEnd() => this.InGame = false;


    void InitialiseRenderers() {
        this.PlayerRenderers = Helper.Players.Select(player =>
            new RendererPair<PlayerControllerB>(player, player.thisPlayerModel)
        );

        this.LandmineRenderers = UnityObject.FindObjectsOfType<Landmine>().Select(landmine =>
            landmine.GetComponent<Renderer>()
        );

        this.TurretRenderers = UnityObject.FindObjectsOfType<Turret>().Select(turret =>
            turret.GetComponent<Renderer>()
        );
    }

    Size GetRendererSize(Renderer renderer, Camera camera) {
        Bounds bounds = renderer.bounds;

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

    void RenderBounds(Camera camera, Renderer renderer, Color colour, Action<Vector3>? action) {
        Vector3 rendererCentrePoint = camera.WorldToEyesPoint(renderer.bounds.center);

        if (rendererCentrePoint.z <= 4.0f) {
            return;
        }

        Helper.DrawOutlineBox(
            rendererCentrePoint,
            this.GetRendererSize(renderer, camera),
            1.0f,
            colour
        );

        action?.Invoke(rendererCentrePoint);
    }

    void RenderBounds(Camera camera, Renderer renderer, Action<Vector3>? action) =>
        this.RenderBounds(camera, renderer, Color.white, action);

    Action<Vector3> RenderPlayer(PlayerControllerB player) => rendererCentrePoint =>
        Helper.DrawLabel(
            rendererCentrePoint,
            $"#{player.playerClientId} {player.playerUsername}"
        );

    Action<Vector3> RenderEnemy(EnemyAI enemy) => rendererCentrePoint =>
        Helper.DrawLabel(
            rendererCentrePoint,
            enemy.enemyType.enemyName,
            Color.red
        );

    Action<Vector3> RenderObject(string name) => rendererCentrePoint =>
        Helper.DrawLabel(
            rendererCentrePoint,
            name,
            Color.yellow
        );
}
