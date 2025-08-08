using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[PrivilegedCommand("land")]
sealed class LandCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot land while travelling to a new level!");
            return;
        }

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 5.0f;

        try {
            startOfRound.StartGameServerRpc();
            await Helper.WaitUntil(() => startOfRound.shipHasLanded, cancellationToken);
        }

        finally {
            Time.timeScale = originalTimeScale;
        }
    }
}
