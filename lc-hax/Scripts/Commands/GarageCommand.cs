using System.Threading;
using System.Threading.Tasks;

[Command("garage")]
sealed class GarageCommand : ICommand {
    static InteractTrigger? GarageTrigger => HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().First(
        interactTrigger => interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes"
    );

    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.RoundManager is not RoundManager roundManager)
            return Task.CompletedTask;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return Task.CompletedTask;
        }

        if (GarageCommand.GarageTrigger is not InteractTrigger garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return Task.CompletedTask;
        }

        garageTrigger.randomChancePercentage = 100;
        garageTrigger.Interact(Helper.LocalPlayer?.transform);
        return Task.CompletedTask;
    }
}
