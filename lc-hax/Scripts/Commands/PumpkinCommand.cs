using System.Linq;
using UnityEngine;
using GameNetcodeStuff;
using Unity.Netcode;
using System.Collections.Generic;

namespace Hax;

public class PumpkinCommand : ICommand {
    private const int DURATION = 60;
    private EnemyAI[] enemyAIs;
    private float instanceTimer = 0;
    private Transform latestEnemy = null;
    private Vector3 latestEnemyPos = Vector3.zero;

    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockables.JACK_O_LANTERN);

        PlaceableShipObject? jack =
            Object.FindObjectsOfType<PlaceableShipObject>()
                  .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockables.JACK_O_LANTERN);

        if (jack == null) {
            return new Result(message: "jack not found!");
        }

        this.GetClosestEnemy(targetPlayer);

        float timer = 0;
        Vector3 bobbing = Vector3.zero;
        GameObject countDown = new();
        _ = countDown.AddComponent<TransientObject>().Init(
            (timeDelta) => {
                this.instanceTimer -= timeDelta;
                timer += timeDelta;
                bobbing = Vector3.up * 0.25f * Mathf.Sin(timer * 2);
                this.GetClosestEnemy(targetPlayer);
                this.latestEnemyPos = targetPlayer.transform.position + targetPlayer.transform.forward;
                if (this.latestEnemy == null) return;

                this.latestEnemyPos = this.latestEnemy.position;

            }, DURATION);

        GameObject g1 = new();
        _ = g1.AddComponent<TransientObject>().Init(
            (x) => {
                Helpers.PlaceObjectAtPosition(
                    jack,
                    targetPlayer.transform.position +
                    (targetPlayer.transform.forward * 2 +
                    targetPlayer.transform.right * 1) +
                    Vector3.up * 2f + bobbing,
                    Quaternion.LookRotation(this.latestEnemyPos - jack.transform.position).eulerAngles + new Vector3(230, -90, 0)).Invoke(x);
            }, DURATION);

        return new Result(true);
    }

    private void GetClosestEnemy(PlayerControllerB targetPlayer) {
        if (this.latestEnemy != null && this.instanceTimer > 0) {
            return;
        }

        this.instanceTimer = 10;

        this.enemyAIs = Object.FindObjectsOfType<EnemyAI>();

        var distance = float.MaxValue;
        foreach (var e in this.enemyAIs) {
            var d = (e.transform.position - targetPlayer.transform.position).sqrMagnitude;
            if (d < distance) {
                this.latestEnemy = e.transform;
                distance = d;
            }
        }
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /pumpkin <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
