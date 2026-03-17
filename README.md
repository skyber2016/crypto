# crypto

A fast, single-binary CLI tool for **AES encryption and decryption** built with .NET 9 (Native AOT).

Manage multiple key profiles and encrypt/decrypt strings from your terminal.

## Installation

### Quick Install (Linux)

```bash
curl -fsSL https://raw.githubusercontent.com/skyber2016/crypto/main/install.sh | bash
```

### Manual Download

Download the latest binary from [GitHub Releases](https://github.com/skyber2016/crypto/releases/latest), then:

```bash
chmod +x crypto-linux-x64
sudo mv crypto-linux-x64 /usr/local/bin/crypto
```

### Uninstall

```bash
curl -fsSL https://raw.githubusercontent.com/skyber2016/crypto/main/uninstall.sh | bash
```

## Usage

### 1. Add a Key Profile

```bash
crypto add <name> -k <secret_key>
```

- `<name>` — Profile name (e.g. `production`, `staging`).
- `-k` / `--key` — Secret key (must be 16, 24, or 32 characters).

### 2. Encrypt

```bash
crypto encrypt <plain_text> --name <profile_name>
# or short form:
crypto encrypt <plain_text> -n <profile_name>
```

With inline key (overrides profile):

```bash
crypto encrypt <plain_text> -k <secret_key>
```

> **Tip:** The last used `--name` is saved, so subsequent calls can omit it.

### 3. Decrypt

```bash
crypto decrypt <base64_text> --name <profile_name>
# or short form:
crypto decrypt <base64_text> -n <profile_name>
```

With inline key:

```bash
crypto decrypt <base64_text> -k <secret_key>
```

### 4. Self-Update

```bash
crypto update
```

Downloads and installs the latest release from GitHub.

## Configuration

Config is stored at `~/.crypto/config.json` with restricted permissions (`600` on Linux).

```json
{
  "LastUsedName": "production",
  "Profiles": {
    "production": "mySecretKey12345!"
  }
}
```

## Build from Source

```bash
dotnet publish -c Release -r linux-x64
# Output: bin/Release/net9.0/linux-x64/publish/crypto-linux-x64
```

## License

MIT
