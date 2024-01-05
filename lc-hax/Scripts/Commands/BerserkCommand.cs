using UnityEngine;

namespace Hax;

[Command("/berserk")]
public class BerserkCommand : ICommand {
    public void Execute(string[] args) =>
        Object.FindObjectsByType<Turret>(FindObjectsSortMode.None)
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
