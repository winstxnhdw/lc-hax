using Hax;

[Command("upright")]
internal class UprightCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.FindObjects<PlaceableShipObject>()
            .ForEach(SetObjectUpright);
    }

    private void SetObjectUpright(PlaceableShipObject shipObject)
    {
        var uprightRotation = shipObject.transform.eulerAngles;

        if (uprightRotation.x == -90.0f) return;

        uprightRotation.x = -90.0f;
        Helper.PlaceObjectAtPosition(shipObject, shipObject.transform.position, uprightRotation);
    }
}