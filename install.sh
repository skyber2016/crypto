#!/usr/bin/env bash
set -euo pipefail

REPO="skyber2016/crypto"
INSTALL_DIR="/usr/local/bin"
BINARY_NAME="crypto"
BINARY_PATH="${INSTALL_DIR}/${BINARY_NAME}"

echo "╔══════════════════════════════════════╗"
echo "║      crypto CLI — Installer          ║"
echo "╚══════════════════════════════════════╝"
echo ""

# Check if already installed
if command -v "$BINARY_NAME" &>/dev/null || [ -f "$BINARY_PATH" ]; then
    CURRENT_VERSION=$("$BINARY_PATH" --version 2>/dev/null || echo "unknown")
    echo "⚠ crypto is already installed (version: ${CURRENT_VERSION})."
    read -rp "→ Reinstall / upgrade? [y/N] " answer
    if [[ ! "$answer" =~ ^[Yy]$ ]]; then
        echo "  → Cancelled."
        exit 0
    fi
    echo ""
fi

# Detect architecture
ARCH=$(uname -m)
OS=$(uname -s | tr '[:upper:]' '[:lower:]')

case "$ARCH" in
    x86_64)  RID="linux-x64" ;;
    aarch64) RID="linux-arm64" ;;
    *)
        echo "✗ Unsupported architecture: $ARCH"
        exit 1
        ;;
esac

case "$OS" in
    linux) ;;
    darwin) RID="osx-${ARCH/#x86_64/x64}" ;;
    *)
        echo "✗ Unsupported OS: $OS"
        exit 1
        ;;
esac

ASSET_NAME="crypto-${RID}"

echo "→ Detected platform: ${RID}"
echo "→ Fetching latest release from GitHub..."

DOWNLOAD_URL=$(curl -fsSL "https://api.github.com/repos/${REPO}/releases/latest" \
    | grep "browser_download_url.*${ASSET_NAME}" \
    | head -1 \
    | cut -d '"' -f 4)

if [ -z "$DOWNLOAD_URL" ]; then
    echo "✗ Could not find asset '${ASSET_NAME}' in the latest release."
    exit 1
fi

echo "→ Downloading ${ASSET_NAME}..."

TMP_FILE=$(mktemp)
curl -fsSL -o "$TMP_FILE" "$DOWNLOAD_URL"

echo "→ Installing to ${INSTALL_DIR}/${BINARY_NAME}..."

if [ -w "$INSTALL_DIR" ]; then
    mv "$TMP_FILE" "${BINARY_PATH}"
else
    sudo mv "$TMP_FILE" "${BINARY_PATH}"
fi

chmod +x "${BINARY_PATH}"

echo ""
echo "✓ crypto installed successfully!"
echo "  Run 'crypto --help' to get started."
