using System.Linq;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class StunCommand : ICommand {
    public void Execute(string[] args) {
        if (!float.TryParse(args[0], out float stunDuration)) {
            Helper.PrintSystem("Usage: /stun <duration>");
            return;
        }

        if (!Helper.Extant(Helper.LocalPlayer, out PlayerControllerB player)) {
            Helper.PrintSystem("Could not find the player!");
            return;
        }

        Physics.OverlapSphere(player.transform.position, 1000.0f, 524288)
               .Select(collider => collider.GetComponent<EnemyAICollisionDetect>())
               .Where(enemy => enemy != null)
               .ToList()
               .ForEach(enemy => enemy.mainScript.SetEnemyStunned(true, stunDuration));
    }
}
