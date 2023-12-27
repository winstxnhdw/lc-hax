using UnityEngine;
using System.Collections;
using GameNetcodeStuff;
using UnityEngine.AI;

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
        Setting.PossessionMod = this;
        this.enabled = false;
    }

    void OnEnable() {
        InputListener.onXPress += this.ToggleRealisticPossession;
        InputListener.onZPress += this.UnPossessEnemy;
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        mousePan.enabled = true;
        rbKeyboard.enabled = true;
    }

    void OnDisable() {
        InputListener.onXPress -= this.ToggleRealisticPossession;
        InputListener.onZPress -= this.UnPossessEnemy;
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        mousePan.enabled = false;
        rbKeyboard.enabled = false;
    }

    private void ToggleRealisticPossession() {
        Setting.RealisticPossessionEnabled = !Setting.RealisticPossessionEnabled;
        Console.Print($"Realistic Possession:{Setting.RealisticPossessionEnabled}");

        if (!this.EnemyToPossess.IsNotNull(out EnemyAI enemy)) return;
        if (!enemy.agent.IsNotNull(out NavMeshAgent agent)) return;
        agent.updatePosition = Setting.RealisticPossessionEnabled;
        agent.updateRotation = Setting.RealisticPossessionEnabled;
    }

    void Update() {
        _ = this.StartCoroutine(this.EndOfFrameCoroutine());
    }

    IEnumerator EndOfFrameCoroutine() {
        yield return new WaitForEndOfFrame();
        this.EndOfFrameUpdate();
    }

    private void EndOfFrameUpdate() {
        if (!this.MousePan.IsNotNull(out MousePan mousePan) ||
            !this.RBKeyboard.IsNotNull(out RBKeyboardMovement rbKeyboard))
            return;

        if (!this.EnemyToPossess.IsNotNull(out EnemyAI enemy) ||
            !Helper.CurrentCamera.IsNotNull(out Camera camera) ||
            !camera.enabled ||
            !Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {

            return;
        }

        if (this.FirstUpdate) {
            if (enemy.GetComponentsInChildren<Collider>().IsNotNull(out Collider[] enemyColliders)) {
                enemyColliders.ForEach(c => c.enabled = false);
            }

            //only works if you enable it before FirstUpdate happens
            if (enemy.agent.IsNotNull(out NavMeshAgent agent)) {
                agent.updatePosition = Setting.RealisticPossessionEnabled;
                agent.updateRotation = Setting.RealisticPossessionEnabled;
            }

            rbKeyboard.Init();
            this.transform.position = enemy.transform.position;
            rbKeyboard.enabled = true;
            mousePan.enabled = true;

        }
        this.UpdateEnemyPositionToHere(enemy);

        camera.transform.position = this.transform.position + (Vector3.up * 2.5f) - (enemy.transform.forward * 2f);
        camera.transform.rotation = this.transform.rotation;

        this.FirstUpdate = false;
    }

    private void UpdateEnemyPositionToHere(EnemyAI enemy) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            return;
        }

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = this.transform.position;
    }

    public void PossessEnemy(EnemyAI enemy) {
        this.UnPossessEnemy();

        this.EnemyToPossess = enemy;
        this.FirstUpdate = true;
    }

    public void UnPossessEnemy() {
        //if previous enemy exists, reset it
        if (this.EnemyToPossess.IsNotNull(out EnemyAI prevEnemy)) {
            prevEnemy.updatePositionThreshold = 1;
            if (prevEnemy.agent.IsNotNull(out NavMeshAgent agent)) {
                agent.updatePosition = true;
                agent.updateRotation = true;
                this.UpdateEnemyPositionToHere(prevEnemy);
                _ = prevEnemy.agent.Warp(prevEnemy.transform.position);
            }

            if (prevEnemy.GetComponentsInChildren<Collider>().IsNotNull(out Collider[] enemyColliders)) {
                enemyColliders.ForEach(c => c.enabled = true);
            }
        }

        this.EnemyToPossess = null;
    }
}
