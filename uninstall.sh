#!/usr/bin/env bash
set -euo pipefail

INSTALL_DIR="/usr/local/bin"
BINARY_NAME="crypto"
CONFIG_DIR="$HOME/.crypto"

echo "╔══════════════════════════════════════╗"
echo "║      crypto CLI — Uninstaller        ║"
echo "╚══════════════════════════════════════╝"
echo ""

# Remove binary
BINARY_PATH="${INSTALL_DIR}/${BINARY_NAME}"
if [ -f "$BINARY_PATH" ]; then
    echo "→ Removing binary at ${BINARY_PATH}..."
    if [ -w "$INSTALL_DIR" ]; then
        rm -f "$BINARY_PATH"
    else
        sudo rm -f "$BINARY_PATH"
    fi
    echo "  ✓ Binary removed."
else
    echo "  ⚠ Binary not found at ${BINARY_PATH}, skipping."
fi

# Remove config
if [ -d "$CONFIG_DIR" ]; then
    read -rp "→ Remove config directory ${CONFIG_DIR}? [y/N] " answer
    if [[ "$answer" =~ ^[Yy]$ ]]; then
        rm -rf "$CONFIG_DIR"
        echo "  ✓ Config removed."
    else
        echo "  → Config kept."
    fi
else
    echo "  ⚠ Config directory not found, skipping."
fi

echo ""
echo "✓ crypto has been uninstalled."
