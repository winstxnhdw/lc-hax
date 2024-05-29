using System;
using Hax;

[DebugCommand("enemies")]
internal class EnemiesCommand : ICommand
{
    public void Execute(StringArray _)
    {
        var enemy = string.Join(", ", Helper.HostileEnemies.Keys);
        Helper.SendNotification("Available Enemies", enemy, false);
        Console.WriteLine(enemy);
    }
}