using Hax;

[Command("/garage")]
public class GarageCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.RoundManager is not RoundManager roundManager) {
            Chat.Print("RoundManager not found!");
            return;
        }

        if (roundManager.currentLevel.levelID is not (int)Level.EXPERIMENTATION) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        HaxObjects.Instance?.InteractTriggers.WhereIsNotNull().ForEach(interactTrigger => {
            if (interactTrigger.name is not "Cube" || interactTrigger.transform.parent.name is not "Cutscenes") return;

            interactTrigger.randomChancePercentage = 100;
            interactTrigger.Interact(Helper.LocalPlayer?.transform);
        });
    }
}
