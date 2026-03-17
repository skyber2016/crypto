using System.CommandLine;
using Crypto.Commands;

var rootCommand = new RootCommand("crypto - AES encryption/decryption CLI tool");

rootCommand.Add(AddCommand.Create());
rootCommand.Add(EncryptCommand.Create());
rootCommand.Add(DecryptCommand.Create());
rootCommand.Add(UpdateCommand.Create());

return await rootCommand.Parse(args).InvokeAsync();
