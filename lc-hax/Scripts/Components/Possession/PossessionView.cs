using System;
using UnityEngine;
using Object = UnityEngine.Object;

sealed class PossessionView : IDisposable {
    EnemyAI Enemy { get; }
    IEnemyController Controller { get; }
    Camera Camera { get; }
    GameObject Root { get; }
    MousePan MousePan { get; }

    internal Quaternion Rotation => this.MousePan.transform.rotation;

    internal PossessionView(EnemyAI enemy, IEnemyController controller, Camera camera) {
        this.Enemy = enemy;
        this.Controller = controller;
        this.Camera = camera;
        this.Root = new GameObject("Hax Possession View");
        this.MousePan = this.Root.AddComponent<MousePan>();
        this.MousePan.SetRotation(this.Camera.transform.rotation);
        this.MousePan.enabled = false;

        Object.DontDestroyOnLoad(this.Root);
    }

    internal void SetManualControl(bool isManual) {
        this.MousePan.enabled = isManual;
        this.MousePan.SetRotation(this.Camera.transform.rotation);
    }

    internal void Prepare(bool inputPaused) => this.MousePan.IsPaused = inputPaused;

    internal void Update() {
        Quaternion rotation = this.Rotation;
        Vector3 offset = this.Controller.CameraOffset;
        Vector3 pivot = this.Enemy.transform.position + (Vector3.up * offset.y);
        Vector3 displacement = rotation * new Vector3(offset.x, 0.0f, offset.z);
        this.Camera.transform.SetPositionAndRotation(pivot + displacement, rotation);

        string? primary = this.Controller.PrimarySkillName;
        string? secondary = this.Controller.SecondarySkillName;
        string separator = string.IsNullOrEmpty(primary) || string.IsNullOrEmpty(secondary) ? "" : "\n";
        Helper.LocalPlayer?.cursorTip.text = string.Concat(primary, separator, secondary);
    }

    public void Dispose() {
        Helper.LocalPlayer?.cursorTip.text = "";
        this.Root.SetActive(false);
        Object.Destroy(this.Root);
    }
}
