using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Spectre.Console;

namespace Crypto.Services;

public static class UpdateService
{
    private const string GitHubApiUrl =
        "https://api.github.com/repos/skyber2016/crypto/releases/latest";

    public static async Task RunAsync()
    {
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";
        AnsiConsole.MarkupLine($"[grey]Current version:[/] [bold]{currentVersion}[/]");

        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("crypto-cli");

        AnsiConsole.Status().Start("Checking for updates...", ctx =>
        {
            ctx.Spinner(Spinner.Known.Dots);
        });

        var json = await http.GetStringAsync(GitHubApiUrl);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var tagName = root.GetProperty("tag_name").GetString()?.TrimStart('v') ?? "0.0.0";

        if (tagName == currentVersion)
        {
            AnsiConsole.MarkupLine("[green]✓ Already up to date.[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[yellow]New version available:[/] [bold]{tagName}[/]");

        var assetName = GetExpectedAssetName();
        string? downloadUrl = null;

        foreach (var asset in root.GetProperty("assets").EnumerateArray())
        {
            var name = asset.GetProperty("name").GetString();
            if (string.Equals(name, assetName, StringComparison.OrdinalIgnoreCase))
            {
                downloadUrl = asset.GetProperty("browser_download_url").GetString();
                break;
            }
        }

        if (string.IsNullOrEmpty(downloadUrl))
        {
            AnsiConsole.MarkupLine($"[red]✗ Asset '{assetName}' not found in release.[/]");
            return;
        }

        var currentExePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("Cannot determine current executable path.");

        var tempPath = currentExePath + ".update";

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"Downloading {assetName}...");
                task.IsIndeterminate = true;

                var bytes = await http.GetByteArrayAsync(downloadUrl);
                await File.WriteAllBytesAsync(tempPath, bytes);

                task.IsIndeterminate = false;
                task.Value = 100;
            });

        File.Move(tempPath, currentExePath, overwrite: true);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            File.SetUnixFileMode(currentExePath,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        }

        AnsiConsole.MarkupLine($"[green]✓ Updated to v{tagName} successfully![/]");
    }

    private static string GetExpectedAssetName()
    {
        var rid = RuntimeInformation.RuntimeIdentifier;
        return $"crypto-{rid}";
    }
}
