using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hax;

public class ESPMod : MonoBehaviour {
    IEnumerable<Renderer> PlayerRenderers { get; set; } = [];
    IEnumerable<Renderer> ScrapRenderers { get; set; } = [];

    bool InGame { get; set; } = false;

    void OnEnable() {
        GameListener.onGameStart += this.ToggleInGame;
        GameListener.onGameEnd += this.ToggleNotInGame;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.ToggleInGame;
        GameListener.onGameEnd -= this.ToggleNotInGame;
    }

    void Start() {
        this.InitialiseRenderers();
    }

    void OnGUI() {
        if (!this.InGame) return;
        this.RenderESP();
    }

    void ToggleInGame() => this.InGame = true;

    void ToggleNotInGame() => this.InGame = false;

    Size GetRendererSize(Renderer renderer, Camera camera) {
        Bounds bounds = renderer.bounds;
        Vector3 min = camera.WorldToScreenPoint(bounds.min);
        Vector3 max = camera.WorldToScreenPoint(bounds.max);

        min.y = Screen.height - min.y;
        max.y = Screen.height - max.y;

        return new Size(
            Mathf.Abs(max.x - min.x),
            Mathf.Abs(max.y - min.y)
        );
    }

    void InitialiseRenderers() {
        this.PlayerRenderers =
            Helper.Players.Select(player => player.TryGetComponent(out Renderer renderer) ? renderer : null)
                          .Where(renderer => renderer is not null)!;

        this.ScrapRenderers =
            Object.FindObjectsOfType<GrabbableObject>()
                  .Select(grabbableObject => grabbableObject.TryGetComponent(out Renderer renderer) ? renderer : null)
                  .Where(renderer => renderer is not null)!;
    }

    void RenderESP() {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        this.PlayerRenderers.ForEach(renderer => {
            Size renderSize = this.GetRendererSize(renderer, camera);
            Vector3 rendererCentrePoint = camera.WorldToScreenPoint(renderer.bounds.center);
            rendererCentrePoint.y = Screen.height - rendererCentrePoint.y;

            Helper.DrawOutlineBox(
                rendererCentrePoint,
                renderSize,
                1.0f
            );
        });

        this.ScrapRenderers.ForEach(renderer => {
            Size renderSize = this.GetRendererSize(renderer, camera);
            Vector3 rendererCentrePoint = camera.WorldToScreenPoint(renderer.bounds.center);
            rendererCentrePoint.y = Screen.height - rendererCentrePoint.y;

            Helper.DrawOutlineBox(
                rendererCentrePoint,
                renderSize,
                1.0f
            );
        });
    }
}
