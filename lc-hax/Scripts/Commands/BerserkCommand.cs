using System;
using Hax;

[Command("/berserk")]
public class BerserkCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) =>
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
