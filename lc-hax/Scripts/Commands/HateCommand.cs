using System.Collections.Generic;
using GameNetcodeStuff;

namespace Hax;

public class HateCommand : ICommand {
    Result Hate(string[] args) {
        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        PromptEnemiesToTarget(targetPlayer);

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Helper.PrintSystem("Usage: /hate <player>");
            return;
        }

        Result result = this.Hate(args);

        if (!result.Success) {
            Helper.PrintSystem(result.Message);
        }
    }

    public static void PromptEnemiesToTarget(PlayerControllerB player) {

        if (!Helper.IsNotNull(RoundManager.Instance, out RoundManager roundManager) ||
            !Helper.IsNotNull(roundManager.SpawnedEnemies, out List<EnemyAI> allEnemies) ||
            !Helper.IsNotNull(Helper.LocalPlayer, out PlayerControllerB localPlayer))
            return;

        _ = Reflector.Target(roundManager).InvokeInternalMethod("RefreshEnemiesList");

        Log($"target {player.playerUsername}. eCount={allEnemies.Count}");

        string enemyNames = "";
        allEnemies.ForEach((e) => {
            //ignore peaceful enemies
            if (Helper.IsNotNull(e as DocileLocustBeesAI, out DocileLocustBeesAI _)
                || Helper.IsNotNull(e as DoublewingAI, out DoublewingAI _))
                return;

            enemyNames += $"{e.name},\n";
            Log($"{e.name}. a");

            e.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            e.targetPlayer = player;

            //thumper
            if (Helper.IsNotNull(e as CrawlerAI, out CrawlerAI crawler)) {
                crawler.BeginChasingPlayerServerRpc((int)player.playerClientId);
                return;
            }

            if (Helper.IsNotNull(e as SandWormAI, out SandWormAI worm)
                || Helper.IsNotNull(e as ForestGiantAI, out ForestGiantAI giant)
                || Helper.IsNotNull(e as MaskedPlayerEnemy, out MaskedPlayerEnemy masked)
                || Helper.IsNotNull(e as SpringManAI, out SpringManAI spring)) {
                e.SwitchToBehaviourState(1);

                if (Helper.IsNotNull(e as ForestGiantAI, out giant)) {
                    giant.chasingPlayer = player;
                    giant.timeSpentStaring = 10;
                    _ = Reflector.Target(giant).SetInternalField("lostPlayerInChase", false);
                }
                return;
            }

            if (Helper.IsNotNull(e as MouthDogAI, out MouthDogAI eyeless)) {
                eyeless.ReactToOtherDogHowl(player.transform.position);
                return;
            }

            if (Helper.IsNotNull(e as FlowermanAI, out FlowermanAI flower)
                || Helper.IsNotNull(e as CentipedeAI, out CentipedeAI centipede)
                || Helper.IsNotNull(e as SandSpiderAI, out SandSpiderAI spider)
                //green purple spore lizard
                || Helper.IsNotNull(e as PufferAI, out PufferAI puffer)
                || Helper.IsNotNull(e as JesterAI, out JesterAI jester)
                || Helper.IsNotNull(e as HoarderBugAI, out HoarderBugAI hoardBug)
                || Helper.IsNotNull(e as RedLocustBees, out RedLocustBees redLocustBees)
                || Helper.IsNotNull(e as NutcrackerEnemyAI, out NutcrackerEnemyAI nutcracker)) {
                e.SwitchToBehaviourState(2);

                if (Helper.IsNotNull(e as FlowermanAI, out flower)) {
                    flower.EnterAngerModeServerRpc(20);
                }

                //spider cannot move 12 distance away from home pos.
                if (Helper.IsNotNull(e as SandSpiderAI, out spider)) {
                    _ = Reflector.Target(spider).SetInternalField("onWall", false);
                    _ = Reflector.Target(spider).SetInternalField("watchFromDistance", false);
                    spider.meshContainer.position = player.transform.position;
                    spider.SyncMeshContainerPositionToClients();
                }

                if (Helper.IsNotNull(e as HoarderBugAI, out hoardBug)) {
                    _ = Reflector.Target(hoardBug).SetInternalField("lostPlayerInChase", false);
                    hoardBug.angryAtPlayer = player;
                    hoardBug.angryTimer = 1000;
                    _ = Reflector.Target(hoardBug).InvokeInternalMethod("SyncNestPositionServerRpc", player.transform.position);
                }

                if (Helper.IsNotNull(e as RedLocustBees, out redLocustBees)) {
                    redLocustBees.hive.isHeld = true;
                }

                if (Helper.IsNotNull(e as NutcrackerEnemyAI, out nutcracker)) {
                    _ = Reflector.Target(nutcracker).SetInternalField("lastSeenPlayerPos", player.transform.position);
                    _ = Reflector.Target(nutcracker).SetInternalField("timeSinceSeeingTarget", 0);
                    nutcracker.SeeMovingThreatServerRpc((int)player.playerClientId);

                }
                return;
            }
        });
    }

    private static void Log(string msg) {
        try {
            Console.Print("HATE", msg);

        }
        catch {

        }
    }
}
