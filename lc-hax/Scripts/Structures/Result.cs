readonly ref struct Result(bool success = false, string? message = null) {
    internal readonly bool Success { get; } = success;
    internal readonly string? Message { get; } = message;
}
