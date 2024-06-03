#region

using Hax;

#endregion

[Command("slam")]
class SlamCommand : ICommand {
    public void Execute(StringArray _) =>
        Helper.FindObjects<SpikeRoofTrap>()
            .ForEach(Spike => Spike.Slam());
}
