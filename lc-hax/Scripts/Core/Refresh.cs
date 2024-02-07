using Hax;
using System.Linq;
using UnityEngine;

public sealed class Refresh : MonoBehaviour {
    private void Awake() {
        if (Helper.LocalPlayer == null || Helper.StartOfRound == null) return;
        if (Helper.StartOfRound.shipHasLanded) this.FillEnemyList();
        this.FillScrapList();
        Destroy(this);
    }

    private void FillEnemyList() =>
        Helper.FindObjects<EnemyAI>().WhereIsNotNull()
            .Where(enemy => enemy.IsSpawned)
            .ForEach(enemy => { _ = Helper.Enemies.Add(enemy); });

    private void FillScrapList() =>
        Helper.FindObjects<GrabbableObject>().WhereIsNotNull()
            .Where(scrap => scrap.IsSpawned)
            .ForEach(scrap => { _ = Helper.Grabbables.Add(scrap); });
}
