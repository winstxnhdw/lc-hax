using Hax;

[Command("berserk")]
class BerserkCommand : ICommand {
    public void Execute(StringArray _) =>
        Helper.FindObjects<Turret>()
              .ForEach(turret => turret.EnterBerserkModeServerRpc(-1));
}
