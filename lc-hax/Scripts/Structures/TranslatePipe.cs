readonly struct TranslatePipe(string sourceLanguage, string targetLanguage) {
    internal string SourceLanguage { get; } = sourceLanguage;
    internal string TargetLanguage { get; } = targetLanguage;
}
