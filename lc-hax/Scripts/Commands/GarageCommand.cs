using System.Threading;
using System.Threading.Tasks;

[Command("garage")]
class GarageCommand : ICommand {
    static InteractTrigger? GarageTrigger => HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().First(
        interactTrigger => interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes"
    );

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.RoundManager is not RoundManager roundManager) return;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        if (GarageCommand.GarageTrigger is not InteractTrigger garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return;
        }

        garageTrigger.randomChancePercentage = 100;
        garageTrigger.Interact(Helper.LocalPlayer?.transform);
    }
}
