using Hax;
using System;

[DebugCommand("enemies")]
internal class EnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        string enemy = string.Join(", ", Helper.HostileEnemies.Keys);
        Helper.SendNotification(title: "Available Enemies", body: enemy, isWarning: false);
        Console.WriteLine(enemy);
    }
}
