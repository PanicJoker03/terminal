# Terminal
Terminal is a program that helps me to resolve my daily work duties

## Features

* Priorities from day
* Look up for past days priorities
* Boss tracking
* Calm operation
* Enterprise Chart
* Client Neccesities Chart

## Backend API

The ASP.NET Core service hosts the terminal workflow without serving a web UI. An Angular client can create an isolated session using `POST /api/terminal-sessions`, send lines using `POST /api/terminal-sessions/{sessionId}/input`, poll buffered output at `GET /api/terminal-sessions/{sessionId}/events?after={sequence}`, and stop it with `DELETE /api/terminal-sessions/{sessionId}`. Session details are available from `GET /api/terminal-sessions/{sessionId}`.

For live terminal output, connect with `@microsoft/signalr` to `/ws/terminal`, invoke `JoinSession(sessionId)`, handle the `terminalEvent` event (`output` or `clear`), and invoke `SendInput(sessionId, text)` for input. A created session may emit output before the hub joins; fetch buffered events from the REST endpoint to catch up.

In development, the OpenAPI document is available at `/openapi/v1.json`. Configure Angular origins using the `Cors:AllowedOrigins` setting; it defaults to `http://localhost:4200`; `appsettings.json` also allows the `aegis-hyper` site origins. Override with `Cors__AllowedOrigins__0=https://my-site` environment variables if the site is served from another origin.

Behind a reverse proxy (e.g. nginx on `aegis-hyper`), forward `/terminal-api/` to the Kestrel port with the prefix stripped and enable WebSocket upgrade for `/ws/terminal`:

```nginx
location /terminal-api/ {
    proxy_pass http://127.0.0.1:5000/;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection $connection_upgrade;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

The browser reports `Status code: (null)` when the request never reaches the API at all (untrusted TLS certificate, missing proxy route, or API not running); accept the certificate for `https://aegis-hyper` in the browser and check `curl -i https://aegis-hyper/terminal-api/api/terminal-sessions`.

Task and priority files use the local `tasks` and `results` directories by default. The container overrides these paths to `/var/lib/terminal-app/tasks` and `/var/lib/terminal-app/results`.

### Jobs database

Job applications are stored in SQLite at `terminal.db` by default for local execution. The container sets `TERMINAL_SQLITE_PATH` to `/var/lib/terminal-app/terminal.db`; mount `/var/lib/terminal-app` as a persistent volume to retain data across container replacement. Set `TERMINAL_SQLITE_PATH` to override the location.

The application no longer reads or writes the legacy jobs CSV. Seed the SQLite database directly from any separately retained backup.

See [TerminalApi.http](TerminalApi.http) for example requests.

### TODO

* update jobs status
* download jobs csv
* mock interview module
* use blazor for frontend
* license

### DISCLAIMER
⚠️ USE AT YOUR OWN RISK ⚠️

#### Author Words

At my current situation, I find that landing a Job is
a task that I qualify as: awful, frustrating and humilliating (interviewed by AI).

Opportunities that never occur hurt my hope and feelings.

This software is what Homer S. will say at that chapter
"Darle un golpe en la cara a Dios".

### Chat last words...

REST endpoints create, inspect, send input to, poll events from, and delete terminal sessions. Each session has its own input queue and output stream, so clients won’t share terminal state. The SignalR hub at /ws/terminal supports live events; the API also buffers recent output for REST polling. See TerminalApi.http and README.md for the client workflow. OpenAPI is available at /openapi/v1.json in Development.