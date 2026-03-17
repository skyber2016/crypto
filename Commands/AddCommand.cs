using System.CommandLine;
using Crypto.Config;
using Spectre.Console;

namespace Crypto.Commands;

public static class AddCommand
{
    public static Command Create()
    {
        var nameArg = new Argument<string>("name") { Description = "Profile name" };
        var keyOption = new Option<string>("--key") { Description = "Secret key", Required = true };
        keyOption.Aliases.Add("-k");

        var command = new Command("add", "Add a new key profile")
        {
            nameArg,
            keyOption
        };

        command.SetAction((result) =>
        {
            var name = result.GetValue(nameArg)!;
            var key = result.GetValue(keyOption)!;

            var config = ConfigManager.Load();
            config.Profiles[name] = key;
            config.LastUsedName ??= name;
            ConfigManager.Save(config);

            AnsiConsole.MarkupLine($"[green]✓[/] Profile [bold]'{name}'[/] saved.");
        });

        return command;
    }
}
