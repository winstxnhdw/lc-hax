using System.Linq;

namespace Hax;

public class ReadyCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.StartOfRound.IsNotNull(out StartOfRound startOfRound)) return;


        Helper.Players?
              .ToList()
              .ForEach(player => Reflector.Target(startOfRound).InvokeInternalMethod("PlayerLoadedServerRpc", 0));
    }
}
