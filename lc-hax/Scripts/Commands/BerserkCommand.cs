#region

using Hax;

#endregion

[Command("berserk")]
class BerserkCommand : ICommand {
    public void Execute(StringArray _) => Helper.FindObjects<Turret>().ForEach(turret => turret.Berserk());
}
