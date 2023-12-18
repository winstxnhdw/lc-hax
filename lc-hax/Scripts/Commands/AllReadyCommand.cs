using System.Linq;
using GameNetcodeStuff;

namespace Hax;
public class AllReadyCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)
            || !Helper.Players.IsNotNull(out PlayerControllerB[] players))
            return;

        for (int i = 0; i < players.Count(); i++) {
            _ = Reflector.Target(startOfRound).InvokeInternalMethod("PlayerLoadedServerRpc", 0);
        }
    }
}
