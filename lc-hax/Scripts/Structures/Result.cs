readonly record struct Result(bool Success = false, string? Message = null) {
    internal bool Success { get; init; } = Success;
    internal string? Message { get; init; } = Message;
}
