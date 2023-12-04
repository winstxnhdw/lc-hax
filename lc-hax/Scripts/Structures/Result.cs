public readonly ref struct Result(bool success = false, string? message = null) {
    public readonly bool Success { get; } = success;
    public readonly string? Message { get; } = message;
}
