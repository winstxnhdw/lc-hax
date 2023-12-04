
using System.Linq;
using UnityEngine;

namespace Hax;

public class UnlockCommand : ICommand {
    public void Execute(string[] args) {
        Object.FindObjectsOfType<DoorLock>().ToList().ForEach(door => door.UnlockDoor());
        Console.Print("SYSTEM", "All doors unlocked!");
    }
}
