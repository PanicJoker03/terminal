#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PUBLISH_DIR="$SCRIPT_DIR/dist/terminal-front/browser"
REMOTE_USER="${DEPLOY_USER:-PanicJoker03}"
REMOTE_HOST="${DEPLOY_HOST:-192.168.1.121}"
REMOTE="$REMOTE_USER@$REMOTE_HOST"
REMOTE_DIR="${DEPLOY_DIR:-/var/www/terminal-app}"
REMOTE_TMP="/tmp/terminal-front-deploy-$$"

cleanup_remote() {
    ssh "$REMOTE" "rm -rf '$REMOTE_TMP'" >/dev/null 2>&1 || true
}

trap cleanup_remote EXIT

tree_hash() {
    # Prints a single SHA-256 that covers every file path and content under the current directory.
    find . -type f -print0 | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum | awk '{print $1}'
}

echo "Building Angular app..."
cd "$SCRIPT_DIR"
if [[ ! -d node_modules ]]; then
    npm ci
fi
rm -rf "$PUBLISH_DIR"
npx ng build --configuration production --base-href /terminal-app/

if [[ ! -f "$PUBLISH_DIR/index.html" ]]; then
    echo "Build output not found at $PUBLISH_DIR (index.html missing)." >&2
    exit 1
fi

echo "Preparing deployment directory on $REMOTE..."
ssh "$REMOTE" "mkdir -p '$REMOTE_TMP'"

echo "Uploading build files..."
tar -C "$PUBLISH_DIR" -cf - . | ssh "$REMOTE" "tar -C '$REMOTE_TMP' -xf -"

echo "Installing application into $REMOTE_DIR..."
ssh -t "$REMOTE" \
    "sudo install -d -m 755 '$REMOTE_DIR' \
     && sudo find '$REMOTE_DIR' -mindepth 1 -delete \
     && sudo cp -a '$REMOTE_TMP/.' '$REMOTE_DIR/' \
     && sudo find '$REMOTE_DIR' -type d -exec chmod 755 {} + \
     && sudo find '$REMOTE_DIR' -type f -exec chmod 644 {} +"

LOCAL_HASH="$(cd "$PUBLISH_DIR" && tree_hash)"
REMOTE_HASH="$(ssh "$REMOTE" "cd '$REMOTE_DIR' && find . -type f -print0 | LC_ALL=C sort -z | xargs -0 sha256sum | sha256sum" | awk '{print $1}')"

if [[ "$LOCAL_HASH" != "$REMOTE_HASH" ]]; then
    echo "Deployment verification failed: file checksums do not match." >&2
    exit 1
fi

echo "Deployment completed successfully."
echo "SHA-256: $LOCAL_HASH"
