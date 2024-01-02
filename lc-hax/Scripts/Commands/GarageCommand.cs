using GameNetcodeStuff;

namespace Hax;

[Command("/garage")]
public class GarageCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            Console.Print("RoundManager not found!");
            return;
        }

        if (roundManager.currentLevel.levelID is not (int)Level.EXPERIMENTATION) {
            Console.Print("You must be in Experimentation to use this command!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            Console.Print("Local player is not found!");
            return;
        }

        HaxObjects.Instance?.InteractTriggers.ForEach(nullableIteractTrigger => {
            if (!nullableIteractTrigger.IsNotNull(out InteractTrigger interactTrigger)) return;
            if (interactTrigger.name is not "Cube" || interactTrigger.transform.parent.name is not "Cutscenes") return;

            interactTrigger.randomChancePercentage = 100;
            interactTrigger.Interact(localPlayer.transform);
        });
    }
}
