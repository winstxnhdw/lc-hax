internal static class State {
    internal static char CommandPrefix { get; set; } = '/';
    internal static int ShovelHitForce { get; set; } = 1;
    internal static bool DisconnectedVoluntarily { get; set; } = false;
    internal static ConnectedLobby? ConnectedLobby { get; set; } = null;
    internal static TranslatePipe? TranslateDetachedState { get; set; } = null;
}
