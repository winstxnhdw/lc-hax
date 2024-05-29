using Hax;

[Command("mergetp")]
internal class MergeTeleportersCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.BuyUnlockable(Unlockable.TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.TELEPORTER);
        Helper.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        Helper.ReturnUnlockable(Unlockable.INVERSE_TELEPORTER);
        if (Helper.GetUnlockable(Unlockable.TELEPORTER) is not PlaceableShipObject teleporter) return;
        if (Helper.GetUnlockable(Unlockable.INVERSE_TELEPORTER) is not PlaceableShipObject inverseTeleporter) return;
        var newRotation = teleporter.transform.eulerAngles;
        newRotation.x = -90.0f;
        Helper.PlaceObjectAtPosition(inverseTeleporter, teleporter.transform.position, newRotation);
    }
}