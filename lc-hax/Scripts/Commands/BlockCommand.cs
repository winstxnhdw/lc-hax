[Command("block")]
internal class BlockCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: block <property>");
            return;
        }

        var result = args[0] switch
        {
            "enemy" => BlockEnemy(),
            "radar" => BlockRadar(),
            _ => "Invalid property!"
        };

        Chat.Print(result);
    }

    private string BlockEnemy()
    {
        Setting.EnableUntargetable = !Setting.EnableUntargetable;

        return $"{(Setting.EnableUntargetable
            ? "Enemies will no longer target you!"
            : "Enemies can now target you!")}";
    }

    private string BlockRadar()
    {
        Setting.EnableBlockRadar = !Setting.EnableBlockRadar;

        return $"{(Setting.EnableBlockRadar
            ? "Blocking radar targets!"
            : "No longer blocking radar targets!")}";
    }
}