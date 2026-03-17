using System.CommandLine;
using Crypto.Config;
using Crypto.Utils;
using Spectre.Console;

namespace Crypto.Commands;

public static class DecryptCommand
{
    public static Command Create()
    {
        var inputArg = new Argument<string>("input") { Description = "Base64-encoded text to decrypt" };
        var nameOption = new Option<string?>("--name") { Description = "Profile name to use" };
        nameOption.Aliases.Add("-n");
        var keyOption = new Option<string?>("--key") { Description = "Inline secret key (overrides profile)" };
        keyOption.Aliases.Add("-k");

        var command = new Command("decrypt", "Decrypt a Base64 string using AES")
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
                var decrypted = EncryptionHelper.DoDecryptAES(input, key);

                AnsiConsole.Write(
                    new Panel(new Text(decrypted, new Style(Color.Cyan1)))
                        .Header("[bold yellow]Decrypted[/]")
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
