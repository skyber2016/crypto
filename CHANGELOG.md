# Changelog

All notable changes to this project will be documented in this file.

## [1.2.0] - 2026-03-17

### Added
- `crypto list` — List all saved key profiles in a table with masked keys.
- `install.sh` — Pre-install check to detect existing installation.
- Uninstall section on landing page.

---

## [1.0.0] - 2026-03-17

### Added
- `crypto add <name> -k <key>` — Add/update a named key profile.
- `crypto encrypt <text> [-n <name>] [-k <key>]` — Encrypt plain text to Base64.
- `crypto decrypt <base64> [-n <name>] [-k <key>]` — Decrypt Base64 to plain text.
- `crypto update` — Self-update from GitHub Releases.
- AES/CTR/NoPadding encryption via BouncyCastle.
- Profile management stored in `~/.crypto/config.json`.
- Auto-remember last used profile name.
- File permissions enforced to `600` on Linux/macOS.
- Native AOT single-file build for `linux-x64`.
