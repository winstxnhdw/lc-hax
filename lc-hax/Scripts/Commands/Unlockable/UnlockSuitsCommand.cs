using System;
using System.Collections.Generic;
using System.Linq;
using Hax;
using UnityEngine;

[Command("unlocksuits")]
internal class UnlockSuitsCommand : ICommand
{
    private static Dictionary<string, int>? Unlockables { get; set; }

    internal Dictionary<string, Unlockable> SuitUnlockables =>
        Enum.GetValues(typeof(Unlockable))
            .Cast<Unlockable>()
            .Where(u => u.ToString().EndsWith("_SUIT"))
            .ToDictionary(suit => suit.ToString().Replace("_SUIT", "").ToLower(), suit => suit);

    public void Execute(StringArray args)
    {
        if (Helper.StartOfRound is not StartOfRound _) return;
        if (Helper.CurrentCamera is not Camera _) return;
        // buy and unlock the suits
        foreach (var suit in SuitUnlockables.Values) Helper.BuyUnlockable(suit);
    }
}