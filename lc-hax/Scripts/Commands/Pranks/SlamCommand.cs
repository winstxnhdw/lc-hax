using Hax;

[Command("slam")]
internal class SlamCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.FindObjects<SpikeRoofTrap>()
            .ForEach(Spike => Spike.Slam());
    }
}