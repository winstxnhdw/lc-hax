using GameNetcodeStuff;
using Hax;

[DebugCommand("fixplayer")]
internal class FixPlayerCommand : ICommand
{
    public void Execute(StringArray _)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        localPlayer.isMovementHindered = 0;
        localPlayer.hinderedMultiplier = 1;
        localPlayer.sourcesCausingSinking = 0;
        localPlayer.sinkingSpeedMultiplier = 0;
    }
}