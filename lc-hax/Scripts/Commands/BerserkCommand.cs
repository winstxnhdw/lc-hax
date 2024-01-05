namespace Hax;

[Command("/berserk")]
public class BerserkCommand : ICommand {
    public void Execute(string[] args) =>
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
