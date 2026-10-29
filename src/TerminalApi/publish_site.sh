#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PUBLISH_DIR="$SCRIPT_DIR/publish"
REMOTE_USER="${DEPLOY_USER:-xenul}"
REMOTE_HOST="${DEPLOY_HOST:-192.168.1.121}"
REMOTE="$REMOTE_USER@$REMOTE_HOST"
REMOTE_DIR="${DEPLOY_DIR:-/var/www/terminal-api}"
REMOTE_TMP="/tmp/terminal-api-deploy-$$"
SERVICE_NAME="${DEPLOY_SERVICE:-terminal-api}"
REMOTE_PORT="${DEPLOY_PORT:-5000}"
RUN_USER="${DEPLOY_RUN_USER:-$REMOTE_USER}"

cleanup_remote() {
    ssh "$REMOTE" "rm -rf '$REMOTE_TMP'" >/dev/null 2>&1 || true
}

trap cleanup_remote EXIT

echo "Publishing terminal-app.dll..."
dotnet publish "$SCRIPT_DIR/TerminalApi.csproj" \
    --configuration Release \
    --output "$PUBLISH_DIR" \
    --no-restore

echo "Preparing deployment directory on $REMOTE..."
ssh "$REMOTE" "mkdir -p '$REMOTE_TMP'"

echo "Uploading published files..."
tar -C "$PUBLISH_DIR" -cf - . | ssh "$REMOTE" "tar -C '$REMOTE_TMP' -xf -"

echo "Installing application into $REMOTE_DIR..."
ssh -t "$REMOTE" \
    "sudo install -d -m 755 '$REMOTE_DIR' /var/lib/terminal-app/tasks /var/lib/terminal-app/results \
     && sudo cp -a '$REMOTE_TMP/.' '$REMOTE_DIR/' \
     && sudo chown -R '$RUN_USER:$RUN_USER' '$REMOTE_DIR' /var/lib/terminal-app \
     && sudo chmod -R u=rwX,go=rX '$REMOTE_DIR' /var/lib/terminal-app"

LOCAL_HASH="$(sha256sum "$PUBLISH_DIR/terminal-app.dll" | awk '{print $1}')"
REMOTE_HASH="$(ssh "$REMOTE" "sha256sum '$REMOTE_DIR/terminal-app.dll'" | awk '{print $1}')"

if [[ "$LOCAL_HASH" != "$REMOTE_HASH" ]]; then
    echo "Deployment verification failed: DLL checksums do not match." >&2
    exit 1
fi

echo "File copy verified (SHA-256 $LOCAL_HASH)."

unit_exists() {
    ssh "$REMOTE" "systemctl cat '$1' >/dev/null 2>&1"
}

if ! unit_exists "$SERVICE_NAME"; then
    FOUND_UNIT="$(ssh "$REMOTE" "grep -ls 'terminal-app\\|terminal-api' /etc/systemd/system/*.service /usr/lib/systemd/system/*.service 2>/dev/null | head -1" || true)"
    if [[ -n "$FOUND_UNIT" ]]; then
        SERVICE_NAME="$(basename "$FOUND_UNIT" .service)"
        echo "Service 'terminal-api' not found, but found existing unit '$SERVICE_NAME' ($FOUND_UNIT); using it."
    else
        echo "No systemd unit for the API exists on $REMOTE. Creating '$SERVICE_NAME.service'..."

        DOTNET_PATH="${DEPLOY_DOTNET:-$(ssh "$REMOTE" 'command -v dotnet || ls /usr/share/dotnet/dotnet /usr/lib/dotnet/dotnet /usr/lib64/dotnet/dotnet "$HOME/.dotnet/dotnet" 2>/dev/null | head -1' || true)}"
        if [[ -z "$DOTNET_PATH" ]]; then
            echo "dotnet runtime not found on $REMOTE. Install the ASP.NET Core runtime there (or set DEPLOY_DOTNET=/path/to/dotnet)." >&2
            exit 1
        fi

        LISTENERS="$(ssh "$REMOTE" "ss -ltnp 2>/dev/null | grep -E ':$REMOTE_PORT\\b' || true")"
        if [[ -n "$LISTENERS" ]]; then
            echo "Port $REMOTE_PORT on $REMOTE is already in use by a process that is not managed by systemd:" >&2
            echo "$LISTENERS" >&2
            echo "That is probably the OLD API started by hand. Stop it (or choose another port with DEPLOY_PORT=<port>) and re-run." >&2
            exit 1
        fi

        UNIT_CONTENT="[Unit]
Description=Terminal API
After=network.target

[Service]
Type=simple
WorkingDirectory=$REMOTE_DIR
ExecStart=$DOTNET_PATH $REMOTE_DIR/terminal-app.dll
Restart=on-failure
RestartSec=5
User=$RUN_USER
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://${DEPLOY_BIND:-127.0.0.1}:$REMOTE_PORT
Environment=TERMINAL_SQLITE_PATH=/var/lib/terminal-app/terminal.db

[Install]
WantedBy=multi-user.target"

        printf '%s\n' "$UNIT_CONTENT" | ssh "$REMOTE" "cat > '$REMOTE_TMP/$SERVICE_NAME.service'"
        ssh -t "$REMOTE" \
            "sudo install -m 644 '$REMOTE_TMP/$SERVICE_NAME.service' '/etc/systemd/system/$SERVICE_NAME.service' \
             && sudo systemctl daemon-reload \
             && sudo systemctl enable '$SERVICE_NAME'"
        echo "Created and enabled /etc/systemd/system/$SERVICE_NAME.service (runs as $RUN_USER)."
    fi
fi

echo "Restarting service '$SERVICE_NAME' (the running process keeps the OLD code until restarted)..."
if ! ssh -t "$REMOTE" "sudo systemctl restart '$SERVICE_NAME' && sleep 3 && systemctl is-active '$SERVICE_NAME'"; then
    echo "Restart failed for '$SERVICE_NAME'. Last log lines:" >&2
    ssh -t "$REMOTE" "sudo journalctl -u '$SERVICE_NAME' -n 30 --no-pager" >&2 || true
    echo "If the unit has another name, run: DEPLOY_SERVICE=<name> $0" >&2
    exit 1
fi

echo "Checking that the RUNNING process uses the new build..."
RUNNING_HASH=""
for _ in 1 2 3 4 5 6 7 8 9 10; do
    RUNNING_HASH="$(ssh "$REMOTE" "curl -fsS http://127.0.0.1:$REMOTE_PORT/api/version" 2>/dev/null \
        | sed -n 's/.*"sha256":"\([0-9a-f]*\)".*/\1/p' || true)"
    [[ -n "$RUNNING_HASH" ]] && break
    sleep 2
done

if [[ -z "$RUNNING_HASH" ]]; then
    echo "Could not read http://127.0.0.1:$REMOTE_PORT/api/version on $REMOTE. Is the service listening on that port (set DEPLOY_PORT)?" >&2
    exit 1
fi

if [[ "$RUNNING_HASH" != "$LOCAL_HASH" ]]; then
    echo "Running process hash $RUNNING_HASH does not match deployed build $LOCAL_HASH." >&2
    echo "The service is likely running from a different directory than $REMOTE_DIR; check its WorkingDirectory/ExecStart." >&2
    exit 1
fi

echo "Deployment completed successfully. The live API is running build $LOCAL_HASH."
