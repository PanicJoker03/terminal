#!/usr/bin/env bash
set -euo pipefail

api_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/src/TerminalApi" && pwd)"
front_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/src/terminal-front" && pwd)"

read -r -p "API username: " Auth__Username
read -r -s -p "API password: " Auth__Password
printf '\n'

if ! command -v openssl >/dev/null 2>&1; then
  printf 'Error: openssl is required to generate the JWT key.\n' >&2
  exit 1
fi

export Auth__Username Auth__Password
export Jwt__Key="$(openssl rand -base64 48)"

api_pid=""
front_pid=""
cleanup() {
  trap - INT TERM EXIT
  [[ -z "$api_pid" ]] || kill "$api_pid" 2>/dev/null || true
  [[ -z "$front_pid" ]] || kill "$front_pid" 2>/dev/null || true
  [[ -z "$api_pid" ]] || wait "$api_pid" 2>/dev/null || true
  [[ -z "$front_pid" ]] || wait "$front_pid" 2>/dev/null || true
}
trap cleanup INT TERM EXIT

(cd "$api_dir" && dotnet run --urls http://localhost:5000) &
api_pid=$!
(cd "$front_dir" && npm start -- --host localhost) &
front_pid=$!

printf 'API: http://localhost:5000\nFrontend: http://localhost:4200\nPress Ctrl+C to stop both.\n'
wait -n "$api_pid" "$front_pid"
