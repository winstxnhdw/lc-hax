
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] args) {
        _ = Object.FindObjectsOfType<DoorLock>().Select(door => {
            door.UnlockDoor();
            return false;
        });

        Console.Print("SYSTEM", "All doors unlocked!");
    }
}
