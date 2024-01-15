using System;
using System.Globalization;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;

[Command("/fatality")]
public class FatalityCommand : ICommand {
    T? GetEnemy<T>() where T : EnemyAI {
        if (Helper.FindObject<T>() is not T enemy) return null;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return null;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        return enemy;
    }

    string? HandleGiant(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<ForestGiantAI>() is not ForestGiantAI forestGiant) {
            return "Enemy has not yet spawned!";
        }

        forestGiant.GrabPlayerServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleJester(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<JesterAI>() is not JesterAI jester) {
            return "Enemy has not yet spawned!";
        }

        jester.KillPlayerServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleMasked(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<MaskedPlayerEnemy>() is not MaskedPlayerEnemy masked) {
            return "Enemy has not yet spawned!";
        }

        masked.KillPlayerAnimationServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleBaboonHawk(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<BaboonBirdAI>() is not BaboonBirdAI baboonHawk) {
            return "Enemy has not yet spawned!";
        }

        baboonHawk.StabPlayerDeathAnimServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleBees(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<RedLocustBees>() is not RedLocustBees bees) {
            return "Enemy has not yet spawned!";
        }

        bees.BeeKillPlayerServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleThumper(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<CrawlerAI>() is not CrawlerAI thumper) {
            return "Enemy has not yet spawned!";
        }

        thumper.EatPlayerBodyServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleEyelessDog(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<MouthDogAI>() is not MouthDogAI eyelessDog) {
            return "Enemy has not yet spawned!";
        }

        eyelessDog.KillPlayerServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleBracken(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<FlowermanAI>() is not FlowermanAI bracken) {
            return "Enemy has not yet spawned!";
        }

        bracken.KillPlayerAnimationServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    string? HandleNutcracker(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<NutcrackerEnemyAI>() is not NutcrackerEnemyAI nutcracker) {
            return "Enemy has not yet spawned!";
        }

        nutcracker.LegKickPlayerServerRpc((int)targetPlayer.playerClientId);
        return null;
    }

    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print($"Unable to find player: {args[0]}!");
            return;
        }

        Dictionary<string, Func<PlayerControllerB, string?>> enemyHandlers = new() {
            { "Forest Giant", this.HandleGiant },
            { "Jester", this.HandleJester },
            { "Masked", this.HandleMasked },
            { "Baboon Hawk", this.HandleBaboonHawk },
            { "Circuit Bees", this.HandleBees },
            { "Thumper", this.HandleThumper },
            { "Eyeless Dog", this.HandleEyelessDog },
            { "Bracken", this.HandleBracken },
            { "Nutcracker", this.HandleNutcracker }
        };

        TextInfo textInfo = new CultureInfo("en-SG").TextInfo;

        string? key = Helper.FuzzyMatch(
            textInfo.ToTitleCase(string.Join(" ", args[1..].ToArray())),
            [.. enemyHandlers.Keys]
        );

        if (key is null) {
            Chat.Print("There are no queryable enemies!");
            return;
        }

        Chat.Print($"Performing {key} fatality on {targetPlayer.playerUsername}..");

        if (enemyHandlers[key](targetPlayer) is string message) {
            Chat.Print(message);
        }
    }
}
