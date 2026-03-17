using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Crypto.Config;

public sealed class CryptoConfig
{
    public string? LastUsedName { get; set; }
    public Dictionary<string, string> Profiles { get; set; } = new();
}

[JsonSerializable(typeof(CryptoConfig))]
internal partial class CryptoConfigContext : JsonSerializerContext;

public static class ConfigManager
{
    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".crypto");

    private static readonly string ConfigPath = Path.Combine(ConfigDir, "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        TypeInfoResolver = CryptoConfigContext.Default
    };

    public static CryptoConfig Load()
    {
        if (!File.Exists(ConfigPath))
            return new CryptoConfig();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize(json, CryptoConfigContext.Default.CryptoConfig) ?? new CryptoConfig();
    }

    public static void Save(CryptoConfig config)
    {
        if (!Directory.Exists(ConfigDir))
        {
            Directory.CreateDirectory(ConfigDir);
            SetUnixPermissions(ConfigDir, isDirectory: true);
        }

        var json = JsonSerializer.Serialize(config, CryptoConfigContext.Default.CryptoConfig);

        // Manual indentation since source-gen context ignores WriteIndented in some scenarios
        File.WriteAllText(ConfigPath, json);
        SetUnixPermissions(ConfigPath, isDirectory: false);
    }

    public static string ResolveKey(string? name, string? inlineKey, CryptoConfig config)
    {
        if (!string.IsNullOrEmpty(inlineKey))
            return inlineKey;

        var profileName = name ?? config.LastUsedName;

        if (string.IsNullOrEmpty(profileName))
            throw new InvalidOperationException(
                "No profile specified. Use --name <name> or add a profile first with 'crypto add'.");

        if (!config.Profiles.TryGetValue(profileName, out var base64Key))
            throw new InvalidOperationException($"Profile '{profileName}' not found in config.");

        if (!string.IsNullOrEmpty(name))
        {
            config.LastUsedName = name;
            Save(config);
        }

        return base64Key;
    }

    private static void SetUnixPermissions(string path, bool isDirectory)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return;

        if (isDirectory)
            File.SetUnixFileMode(path,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        else
            File.SetUnixFileMode(path,
                UnixFileMode.UserRead | UnixFileMode.UserWrite);
    }
}
