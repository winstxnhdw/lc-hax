#region

using Hax;
using UnityEngine;

#endregion

public enum CaveDwellerState {
    Idle = 1,
    Searching = 2,
    Attacking = 3
}


class CaveDwellerController : IEnemyController<CaveDwellerAI> {
    readonly Vector3 camOffset = new(0, 3.2f, -4f);




}
