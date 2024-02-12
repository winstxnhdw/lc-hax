using Hax;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DebugCommand("enemies")]
internal class EnemiesCommand : ICommand {

    public void Execute(StringArray _) {
        string enemy = string.Join(", ", Helper.HostileEnemies.Keys);
        Helper.SendNotification("Available Enemies", enemy, false);
        Console.WriteLine(enemy);
    }
}
