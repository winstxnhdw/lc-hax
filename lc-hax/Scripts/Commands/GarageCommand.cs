using System.Threading;
using System.Threading.Tasks;

[Command("garage")]
sealed class GarageCommand : ICommand {
    static InteractTrigger? GarageTrigger => HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().First(
        interactTrigger => interactTrigger != null && interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes"
    );

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.RoundManager is not { } roundManager) return Task.CompletedTask;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return Task.CompletedTask;
        }

        if (GarageCommand.GarageTrigger is not { } garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return Task.CompletedTask;
        }

        garageTrigger.randomChancePercentage = 100;
        garageTrigger.Interact(Helper.LocalPlayer?.transform);
        return Task.CompletedTask;
    }
}
