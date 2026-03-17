using System.CommandLine;
using Crypto.Config;
using Spectre.Console;

namespace Crypto.Commands;

public static class ListCommand
{
    public static Command Create()
    {
        var command = new Command("list", "List all saved key profiles");

        command.SetAction((result) =>
        {
            var config = ConfigManager.Load();

            if (config.Profiles.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No profiles found.[/] Use [bold]crypto add[/] to create one.");
                return;
            }

            var table = new Table()
                .BorderColor(Color.Grey)
                .AddColumn(new TableColumn("[bold]Name[/]").LeftAligned())
                .AddColumn(new TableColumn("[bold]Key[/]").LeftAligned())
                .AddColumn(new TableColumn("[bold]Active[/]").Centered());

            foreach (var (name, key) in config.Profiles)
            {
                var masked = key.Length > 6
                    ? key[..3] + new string('*', key.Length - 6) + key[^3..]
                    : new string('*', key.Length);

                var active = name == config.LastUsedName ? "[green]✓[/]" : "";
                table.AddRow(name, masked, active);
            }

            AnsiConsole.Write(table);
        });

        return command;
    }
}
