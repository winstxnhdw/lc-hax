// using System.Linq;
// using UnityEngine;
// using GameNetcodeStuff;

// namespace Hax;

// public class PumpkinCommand : ICommand {
//     private const int DURATION = 60;
//     private EnemyAI[] enemyAIs;
//     private float instanceTimer = 0;
//     private Transform latestEnemy = null;
//     private Vector3 toOutwardsPos = Vector3.zero;

//     Result TeleportPlayerToRandom(string[] args) {
//         if (!Helper.IsNotNull(Helper.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
//             return new Result(message: "Player not found!");
//         }

//         Helper.BuyUnlockable(Unlockable.JACK_O_LANTERN);

//         PlaceableShipObject? jack =
//             Object.FindObjectsOfType<PlaceableShipObject>()
//                   .FirstOrDefault(placeableObject => placeableObject.unlockableID == (int)Unlockable.JACK_O_LANTERN);

//         if (jack == null) {
//             return new Result(message: "jack not found!");
//         }

//         this.GetClosestEnemy(targetPlayer);

//         float timer = 0;
//         Vector3 bobbing = Vector3.zero;
//         Vector3 toPlayerPos = targetPlayer.transform.position +
//                     ((targetPlayer.transform.forward * 2) +
//                     (targetPlayer.transform.right * 1)) +
//                     (Vector3.up * 2f);
//         float pingpongValue = 0;
//         float toEnemyMaxDist = 5;
//         GameObject countDown = new();
//         _ = countDown.AddComponent<TransientBehaviour>().Init(
//             (timeDelta) => {
//                 toPlayerPos = targetPlayer.transform.position +
//                     ((targetPlayer.transform.forward * 2) +
//                     (targetPlayer.transform.right * 1)) +
//                     (Vector3.up * 2f);

//                 this.instanceTimer -= timeDelta;
//                 timer += timeDelta;
//                 bobbing = Vector3.up * 0.25f * Mathf.Sin(timer * 2);
//                 pingpongValue = Mathf.PingPong(timer, 1);
//                 this.GetClosestEnemy(targetPlayer);
//                 this.toOutwardsPos = toPlayerPos;
//                 if (this.latestEnemy == null) return;

//                 this.toOutwardsPos = toPlayerPos + ((this.latestEnemy.position - toPlayerPos).normalized * toEnemyMaxDist);

//             }, DURATION);

//         GameObject g1 = new();
//         _ = g1.AddComponent<TransientBehaviour>().Init(
//             (x) => {
//                 Helper.PlaceObjectAtPosition(
//                     jack,
//                     Vector3.Lerp(toPlayerPos, this.toOutwardsPos, pingpongValue) + bobbing,
//                     Quaternion.LookRotation(targetPlayer.transform.position - jack.transform.position, Vector3.up).eulerAngles + new Vector3(220, -90, 0));
//             }, DURATION);

//         return new Result(true);
//     }

//     private void GetClosestEnemy(PlayerControllerB targetPlayer) {
//         if (this.latestEnemy != null && this.instanceTimer > 0) {
//             return;
//         }

//         this.instanceTimer = 10;

//         this.enemyAIs = Object.FindObjectsOfType<EnemyAI>();

//         float distance = float.MaxValue;
//         foreach (EnemyAI e in this.enemyAIs) {
//             float d = (e.transform.position - targetPlayer.transform.position).sqrMagnitude;
//             if (d < distance) {
//                 this.latestEnemy = e.transform;
//                 distance = d;
//             }
//         }
//     }

//     public void Execute(string[] args) {
//         if (args.Length < 1) {
//             Console.Print("Usage: /pumpkin <player>");
//             return;
//         }

//         Result result = this.TeleportPlayerToRandom(args);

//         if (!result.Success) {
//             Console.Print(result.Message);
//         }
//     }
// }
