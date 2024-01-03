using UnityEngine;

namespace Hax;

[Command("/berserk")]
public class BerserkCommand : ICommand {
    public void Execute(string[] args) => Object.FindObjectsOfType<Turret>().ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
