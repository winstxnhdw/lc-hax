using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

//must be enabled and disabled by phantommod. Or else phantom mod can break, and this break, what nots.
public sealed class PossessionMod : MonoBehaviour {
    public bool Possessing => this.EnemyToPossess != null;
    EnemyAI? EnemyToPossess { get; set; } = null;
    bool FirstUpdate { get; set; } = true;
    RBKeyboardMovement? RBKeyboard { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    void Awake() {
        this.RBKeyboard = this.gameObject.AddComponent<RBKeyboardMovement>();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        Settings.PossessionMod = this;
        this.enabled = false;
    }

    void OnEnable() {
        InputListener.onZPress += this.UnPossessEnemy;
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        mousePan.enabled = true;
        rbKeyboard.enabled = true;
    }

    void OnDisable() {
        InputListener.onZPress -= this.UnPossessEnemy;
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        mousePan.enabled = false;
        rbKeyboard.enabled = false;
    }

    void Update() {
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        if (!this.EnemyToPossess.IsNotNull(out EnemyAI enemy) ||
            !Helper.CurrentCamera.IsNotNull(out Camera camera) ||
            !camera.enabled ||
            !Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) ||
            !enemy.GetComponentInChildren<Rigidbody>().IsNotNull(out Rigidbody _)) {

            return;
        }

        if (this.FirstUpdate) {
            rbKeyboard.Init();
            this.transform.position = enemy.transform.position;
            rbKeyboard.enabled = true;
            mousePan.enabled = true;

        }

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = this.transform.position;

        camera.transform.position = this.transform.position + (-enemy.transform.forward) + (Vector3.up * 2);
        camera.transform.rotation = this.transform.rotation;


        this.FirstUpdate = false;
    }

    public void PossessEnemy(EnemyAI enemy) {
        this.EnemyToPossess = enemy;
        this.FirstUpdate = true;
    }

    public void UnPossessEnemy() {
        if (this.EnemyToPossess.IsNotNull(out EnemyAI enemy)) {
            enemy.updatePositionThreshold = 1;
        }
        this.EnemyToPossess = null;
    }
}
