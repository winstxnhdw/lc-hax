namespace Hax;

public static partial class Helpers {
    public static void BuyUnlockable(Unlockables unlockable) {
        HUDManager? hudManager = HaxObjects.Instance?.HUDManager?.Object;
        Terminal? terminal = hudManager == null ? null : Reflector.Target(hudManager).GetInternalField<Terminal>("terminalScript");

        if (terminal == null) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)unlockable, terminal.groupCredits);
    }
}
