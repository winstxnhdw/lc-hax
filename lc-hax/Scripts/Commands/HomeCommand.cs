
namespace Hax;

public class HomeCommand : ICommand {
    public void Execute(string[] args) {
        StartOfRound.Instance.ForcePlayerIntoShip();
    }
}
