namespace Hax;

public static partial class Helpers {
    public static void BuyUnlockable(Unlockables unlockable) {
        if (!Helpers.Extant(Helpers.Terminal, out Terminal terminal)) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)unlockable, terminal.groupCredits);
    }
}
