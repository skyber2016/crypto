using System.CommandLine;
using Crypto.Config;
using Spectre.Console;

namespace Crypto.Commands;

public static class RemoveCommand
{
    public static Command Create()
    {
        var nameArg = new Argument<string?>("name")
        {
            Description = "Profile name to remove",
            Arity = ArgumentArity.ZeroOrOne
        };

        var allOption = new Option<bool>("-a", "--all") { Description = "Remove all profiles" };

        var command = new Command("remove", "Remove a key profile or all profiles")
        {
            nameArg,
            allOption
        };

        command.SetAction((result) =>
        {
            var name = result.GetValue(nameArg);
            var removeAll = result.GetValue(allOption);
            var config = ConfigManager.Load();

            if (removeAll)
            {
                if (config.Profiles.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No profiles to remove.[/]");
                    return;
                }

                var confirm = AnsiConsole.Confirm(
                    $"[red]Remove ALL {config.Profiles.Count} profile(s)?[/]", defaultValue: false);

                if (!confirm)
                {
                    AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
                    return;
                }

                config.Profiles.Clear();
                config.LastUsedName = null;
                ConfigManager.Save(config);

                AnsiConsole.MarkupLine("[green]✓[/] All profiles removed.");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                AnsiConsole.MarkupLine("[red]✗[/] Specify a profile name or use [bold]--all[/].");
                return;
            }

            if (!config.Profiles.ContainsKey(name))
            {
                AnsiConsole.MarkupLine($"[red]✗[/] Profile [bold]'{name}'[/] not found.");
                return;
            }

            config.Profiles.Remove(name);

            if (config.LastUsedName == name)
                config.LastUsedName = null;

            ConfigManager.Save(config);

            AnsiConsole.MarkupLine($"[green]✓[/] Profile [bold]'{name}'[/] removed.");
        });

        return command;
    }
}
