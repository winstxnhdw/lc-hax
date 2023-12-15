using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class StunCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /stun <duration>");
            return;
        }

        if (!float.TryParse(args[0], out float stunDuration)) {
            Console.Print("Invalid duration!");
            return;
        }

        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
            Console.Print("Could not find the player!");
            return;
        }

        Physics.OverlapSphere(player.transform.position, float.MaxValue, 524288)
               .Select(collider => collider.GetComponent<EnemyAICollisionDetect>())
               .Where(enemy => enemy != null)
               .ToList()
               .ForEach(enemy => enemy.mainScript.SetEnemyStunned(true, stunDuration));
    }
}
