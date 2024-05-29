internal readonly ref struct Result(bool success = false, string? message = null)
{
    internal bool Success { get; } = success;
    internal string? Message { get; } = message;
}