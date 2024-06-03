readonly record struct TranslatePipe {
    internal required string SourceLanguage { get; init; }
    internal required string TargetLanguage { get; init; }
}
