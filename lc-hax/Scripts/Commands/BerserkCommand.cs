using Hax;

[Command("berserk")]
internal class BerserkCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.FindObjects<Turret>().ForEach(turret => turret.Berserk());
    }
}