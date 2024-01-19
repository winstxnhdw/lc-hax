using Hax;

[Command("/garage")]
public class GarageCommand : ICommand {
    InteractTrigger? GarageTrigger => HaxObjects.Instance?.InteractTriggers?.WhereIsNotNull().First(
        interactTrigger => interactTrigger.name is "Cube" && interactTrigger.transform.parent.name is "Cutscenes"
    );

    public void Execute(StringArray _) {
        if (Helper.RoundManager is not RoundManager roundManager) return;
        if (roundManager.currentLevel.levelID is not 0) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        if (this.GarageTrigger is not InteractTrigger garageTrigger) {
            Chat.Print("Garage trigger is not found!");
            return;
        }

        garageTrigger.randomChancePercentage = 100;
        garageTrigger.Interact(Helper.LocalPlayer?.transform);
    }
}
