using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class StunCommand : ICommand {
    public void Execute(string[] args) {
        bool isValidFloat = float.TryParse(args[0], out float stunDuration);

        if (!isValidFloat) {
            Console.Print("SYSTEM", "Usage: /stun <duration>");
            return;
        }

        PlayerControllerB? player = Helpers.LocalPlayer;

        if (player == null) {
            Console.Print("SYSTEM", "Could not find the player!");
            return;
        }

        Physics.OverlapSphere(player.transform.position, 1000.0f, 524288)
               .Select(collider => collider.GetComponent<EnemyAICollisionDetect>())
               .Where(enemy => enemy != null)
               .ToList()
               .ForEach(enemy => enemy.mainScript.SetEnemyStunned(true, stunDuration));
    }
}
