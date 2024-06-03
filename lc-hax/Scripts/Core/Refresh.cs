#region

using System.Linq;
using Hax;
using UnityEngine;

#endregion

sealed class Refresh : MonoBehaviour {
    void Start() {
        if (Helper.LocalPlayer == null) return;
        if (Helper.StartOfRound?.shipHasLanded is true) this.FillEnemyList();
        this.FillScrapList();
        Destroy(this);
    }

    void FillEnemyList() =>
        Helper.FindObjects<EnemyAI>().WhereIsNotNull()
            .Where(enemy => enemy.IsSpawned)
            .ForEach(enemy => { _ = Helper.Enemies.Add(enemy); });

    void FillScrapList() =>
        Helper.FindObjects<GrabbableObject>().WhereIsNotNull()
            .Where(scrap => scrap.IsSpawned)
            .ForEach(scrap => { _ = Helper.Grabbables.Add(scrap); });
}
