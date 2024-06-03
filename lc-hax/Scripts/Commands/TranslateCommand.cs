#region

using Hax;

#endregion

[Command("translate")]
class TranslateCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length < 3) {
            Chat.Print("Usages:",
                "translate <language> <language> <text>",
                "translate <language> <language> -d"
            );

            return;
        }

        if (args[0] is not string source || args[1] is not string target) {
            Chat.Print("Invalid source or target language!");
            return;
        }

        if (args[2] is "-d") {
            State.TranslateDetachedState = new TranslatePipe() {
                SourceLanguage = source,
                TargetLanguage = target
            };

            Helper.CreateToggleableComponent<WaitForGameEndBehaviour>("Translate Detached")?
                .AddActionAfter(() => State.TranslateDetachedState = null);
        }

        else
            Helper.Translate(source, target, string.Join(' ', args[2..]));
    }
}
