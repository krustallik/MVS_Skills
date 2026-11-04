# Development Setup

## Requirements

- .NET 8 SDK
- `dotnet-ef` version 9.0.2 for the checked-in Entity Framework migrations

Install the EF command-line tool if it is not already available:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.2
```

## Start with an isolated SQLite database

Run these commands from the repository root in PowerShell. The connection-string override keeps development data outside the repository and avoids modifying `MVC/site.db`.

```powershell
dotnet restore MVC/MVC.csproj
dotnet build MVC/MVC.csproj --no-restore

$demoDb = Join-Path $env:TEMP 'mvs-skills-local.sqlite'
$env:ConnectionStrings__DefaultConnection = "Data Source=$demoDb"

dotnet ef database update --project MVC/MVC.csproj --startup-project MVC/MVC.csproj
dotnet run --project MVC/MVC.csproj --urls http://127.0.0.1:5179
```

Open <http://127.0.0.1:5179/>. Register a local account to try the regular-user flow. The repository does not include a seeded administrator account; the admin area requires an account assigned the `Admin` role.

Stop the app with `Ctrl+C`. The database file remains in the temporary directory and can be removed after the app has stopped:

```powershell
Remove-Item -LiteralPath $demoDb
```

The environment-variable override applies only to the current PowerShell session. Start a new session and set it again before running migrations or the app.

## Database migrations

After changing the Entity Framework models, create a migration from the repository root:

```powershell
dotnet ef migrations add DescribeYourChange --project MVC/MVC.csproj --startup-project MVC/MVC.csproj
```

Apply migrations to the isolated database with `dotnet ef database update` as shown above. Keep the connection-string override set for both commands.

## Troubleshooting

- **`dotnet ef` is not recognized:** install the global tool above, then open a new terminal.
- **The database is locked:** stop other running app instances that use the same SQLite file and retry.
- **The selected port is busy:** choose another local port in the `--urls` argument.
- **A database appears inside the repository:** check that `ConnectionStrings__DefaultConnection` is set in the current terminal before starting the app.
