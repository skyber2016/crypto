using System.CommandLine;
using Crypto.Commands;
using System.Reflection;

var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";
var rootCommand = new RootCommand($"crypto v{version} - AES encryption/decryption CLI tool");

rootCommand.Add(AddCommand.Create());
rootCommand.Add(ListCommand.Create());
rootCommand.Add(EncryptCommand.Create());
rootCommand.Add(DecryptCommand.Create());
rootCommand.Add(RemoveCommand.Create());
rootCommand.Add(UpdateCommand.Create());

return await rootCommand.Parse(args).InvokeAsync();
