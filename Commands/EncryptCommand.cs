using System.CommandLine;
using Crypto.Config;
using Crypto.Utils;
using Spectre.Console;

namespace Crypto.Commands;

public static class EncryptCommand
{
    public static Command Create()
    {
        var inputArg = new Argument<string>("input") { Description = "Plain text to encrypt" };
        var nameOption = new Option<string?>("--name") { Description = "Profile name to use" };
        nameOption.Aliases.Add("-n");
        var keyOption = new Option<string?>("--key") { Description = "Inline secret key (overrides profile)" };
        keyOption.Aliases.Add("-k");

        var command = new Command("encrypt", "Encrypt a string using AES")
        {
            inputArg,
            nameOption,
            keyOption
        };

        command.SetAction((result) =>
        {
            var input = result.GetValue(inputArg)!;
            var name = result.GetValue(nameOption);
            var inlineKey = result.GetValue(keyOption);

            try
            {
                var config = ConfigManager.Load();
                var key = ConfigManager.ResolveKey(name, inlineKey, config);
                var encrypted = EncryptionHelper.DoEncryptAES(input, key);

                AnsiConsole.Write(
                    new Panel(new Text(encrypted, new Style(Color.Green)))
                        .Header("[bold yellow]Encrypted[/]")
                        .BorderColor(Color.Grey));
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]✗ {ex.Message}[/]");
            }
        });

        return command;
    }
}
