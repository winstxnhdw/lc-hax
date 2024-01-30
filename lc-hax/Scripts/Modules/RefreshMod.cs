using Hax;
using UnityEngine;

public sealed class RefreshMod : MonoBehaviour {

    private void Awake() {
        if (Helper.LocalPlayer is null) return;
        if (Helper.StartOfRound is null) return;
        if (Helper.StartOfRound.shipHasLanded) {
            this.FillEnemyList();
        }
            this.FillScrapList();
        Destroy(this.gameObject);
    }

    private void FillEnemyList() {
        EnemyAI[] Enemies = Helper.FindObjects<EnemyAI>();
        foreach (EnemyAI Enemy in Enemies) {
            // check if they are actively online.
            if (Enemy.IsSpawned) {
                _ = Helper.Enemies.Add(Enemy);
            }
        }
    }

    private void FillScrapList() {
        GrabbableObject[] Scraps = Helper.FindObjects<GrabbableObject>();
        foreach (GrabbableObject Scrap in Scraps) {
            // check if they are actively online.
            if (Scrap.IsSpawned) {
                _ = Helper.Grabbables.Add(Scrap);
            }
        }
    }
}
