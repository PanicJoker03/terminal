FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
VOLUME ["/var/lib/terminal-app"]
ENV TERMINAL_SQLITE_PATH=/var/lib/terminal-app/terminal.db
ENV FilePaths__Tasks=/var/lib/terminal-app/tasks
ENV FilePaths__Results=/var/lib/terminal-app/results
ENTRYPOINT ["dotnet", "terminal-app.dll"]
