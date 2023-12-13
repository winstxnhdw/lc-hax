using UnityEngine;

namespace Hax;

public class TimeScaleCommand : ICommand {
    public void Execute(string[] args) {
        Time.timeScale = float.Parse(args[0]);
    }
}
