using System.CommandLine;
using Crypto.Commands;
using Crypto.Config;
using Crypto.Utils;
using Spectre.Console;
using System.Reflection;

var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";
var rootCommand = new RootCommand($"crypto v{version} - AES encryption/decryption CLI tool");

// Short options for encrypt/decrypt
var encryptOption = new Option<string?>("--encrypt") { Description = "Encrypt a string (shorthand for 'crypto encrypt')" };
encryptOption.Aliases.Add("-e");
var decryptOption = new Option<string?>("--decrypt") { Description = "Decrypt a Base64 string (shorthand for 'crypto decrypt')" };
decryptOption.Aliases.Add("-d");
var nameOption = new Option<string?>("--name") { Description = "Profile name to use" };
nameOption.Aliases.Add("-n");
var keyOption = new Option<string?>("--key") { Description = "Inline secret key (overrides profile)" };
keyOption.Aliases.Add("-k");

rootCommand.Add(encryptOption);
rootCommand.Add(decryptOption);
rootCommand.Add(nameOption);
rootCommand.Add(keyOption);

rootCommand.Add(AddCommand.Create());
rootCommand.Add(ListCommand.Create());
rootCommand.Add(EncryptCommand.Create());
rootCommand.Add(DecryptCommand.Create());
rootCommand.Add(RemoveCommand.Create());
rootCommand.Add(UpdateCommand.Create());

rootCommand.SetAction((result) =>
{
    var encryptInput = result.GetValue(encryptOption);
    var decryptInput = result.GetValue(decryptOption);
    var name = result.GetValue(nameOption);
    var inlineKey = result.GetValue(keyOption);

    if (encryptInput is null && decryptInput is null)
    {
        AnsiConsole.MarkupLine($"[bold yellow]crypto[/] v{version} — AES encryption/decryption CLI tool");
        AnsiConsole.MarkupLine("Use [green]--help[/] to see available commands and options.");
        return;
    }

    try
    {
        var config = ConfigManager.Load();
        var key = ConfigManager.ResolveKey(name, inlineKey, config);

        if (encryptInput is not null)
        {
            var encrypted = EncryptionHelper.DoEncryptAES(encryptInput, key);
            AnsiConsole.Write(
                new Panel(new Text(encrypted, new Style(Color.Green)))
                    .Header("[bold yellow]Encrypted[/]")
                    .BorderColor(Color.Grey));
        }
        else if (decryptInput is not null)
        {
            var decrypted = EncryptionHelper.DoDecryptAES(decryptInput, key);
            AnsiConsole.Write(
                new Panel(new Text(decrypted, new Style(Color.Cyan1)))
                    .Header("[bold yellow]Decrypted[/]")
                    .BorderColor(Color.Grey));
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]✗ {ex.Message}[/]");
    }
});

return await rootCommand.Parse(args).InvokeAsync();
