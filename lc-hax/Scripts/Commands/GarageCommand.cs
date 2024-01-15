using System;
using Hax;

[Command("/garage")]
public class GarageCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            Chat.Print("RoundManager not found!");
            return;
        }

        if (roundManager.currentLevel.levelID is not (int)Level.EXPERIMENTATION) {
            Chat.Print("You must be in Experimentation to use this command!");
            return;
        }

        HaxObjects.Instance?.InteractTriggers.ForEach(nullableIteractTrigger => {
            if (!nullableIteractTrigger.IsNotNull(out InteractTrigger interactTrigger)) return;
            if (interactTrigger.name is not "Cube" || interactTrigger.transform.parent.name is not "Cutscenes") return;

            interactTrigger.randomChancePercentage = 100;
            interactTrigger.Interact(Helper.LocalPlayer?.transform);
        });
    }
}
