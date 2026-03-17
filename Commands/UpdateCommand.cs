using System.CommandLine;
using Crypto.Services;
using Spectre.Console;

namespace Crypto.Commands;

public static class UpdateCommand
{
    public static Command Create()
    {
        var command = new Command("update", "Download the latest version from GitHub");

        command.SetAction(async (result) =>
        {
            try
            {
                await UpdateService.RunAsync();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ Update failed: {ex.Message}[/]");
            }
        });

        return command;
    }
}
