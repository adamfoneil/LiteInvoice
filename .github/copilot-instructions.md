# LiteInvoice Development Environment

LiteInvoice is a .NET 9 Blazor Server application for creating invoices, hosted on DigitalOcean with a PostgreSQL database. The application uses Entity Framework Core, SignalR, and various UI libraries including Radzen.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

- Bootstrap, build, and test the repository:
  - Install .NET 9 SDK: `curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --install-dir ~/.dotnet`
  - Add .NET 9 to PATH: `export PATH="$HOME/.dotnet:$PATH"`
  - Install Docker if needed: `docker --version` (usually pre-installed)
  - Set up PostgreSQL database: `docker run -d --name postgres-local -e POSTGRES_USER=postgresUser -e POSTGRES_PASSWORD=postgresPwd -e POSTGRES_DB=pgdb -p 5433:5432 -v postgres-data:/var/lib/postgresql/data postgres:latest` -- takes 6 seconds
  - Install EF Core tools: `dotnet tool install --global dotnet-ef` -- takes 3 seconds
  - Restore packages: `dotnet restore LiteInvoice.slnx` -- takes 18 seconds. Set timeout to 30+ seconds.
  - Build solution: `dotnet build LiteInvoice.slnx --configuration Release` -- takes 15 seconds. Set timeout to 30+ seconds.
  - Run database migrations: `cd LiteInvoice.Database && dotnet ef database update` -- takes 7 seconds. Set timeout to 15+ seconds.

- Run the application:
  - ALWAYS run the bootstrapping steps first.
  - Start application: `cd BlazorApp && dotnet run` -- application starts in 4 seconds and listens on http://localhost:5041
  - Application uses Blazor Server with SignalR - the WebSocket connection should establish automatically

## Validation

- ALWAYS manually validate any new code by accessing the running application at http://localhost:5041
- ALWAYS run through at least one complete end-to-end scenario after making changes:
  - Navigate to the homepage (should show "LiteInvoice" heading and "This is for creating invoices")
  - Click "Register" link and verify the registration form loads
  - Click "Home" to return to homepage
  - Verify no JavaScript console errors and SignalR connection establishes
- Build produces 20 nullable reference warnings - this is normal and expected
- No unit tests exist in the project - manual testing via browser is the only validation method
- Database connectivity is verified automatically on application startup (check logs for EF migration messages)

## Common Tasks

The following are outputs from frequently run commands. Reference them instead of viewing, searching, or running bash commands to save time.

### Project Structure
```
LiteInvoice/
├── .github/workflows/          # CI/CD workflows
├── BlazorApp/                  # Main Blazor Server web application
├── LiteInvoice.Database/       # EF Core DbContext and entities
├── LiteInvoice.slnx           # Solution file (.slnx format for .NET 9)
├── README.md                   # Project documentation  
├── pg-setup-docker.txt        # PostgreSQL Docker setup commands
└── version.json               # Version tracking
```

### Key Dependencies (from BlazorApp.csproj)
- .NET 9.0
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.4
- Microsoft.EntityFrameworkCore.SqlServer 9.0.4 (Note: app actually uses PostgreSQL)
- Radzen.Blazor 6.6.1 (UI components)
- Coravel 6.0.2 (task scheduling)
- AO.Blazor.CurrentUser 1.0.2 (user management)

### Database Configuration
- PostgreSQL on localhost:5433
- Database name: liteinvoice  
- User: postgresUser / Password: postgresPwd
- Connection string in appsettings.json: `Host=localhost;Port=5433;Database=liteinvoice;Username=postgresUser;Password=postgresPwd`

### Build Warnings (Expected)
The build produces 20 nullable reference warnings in Blazor components - these are expected and do not prevent successful compilation:
- CS8601: Possible null reference assignment
- CS8602: Dereference of a possibly null reference

### Common Issues and Solutions
- **"The element <Solution> is unrecognized"**: You need .NET 9 SDK. The .slnx format requires .NET 9+
- **"dotnet-ef does not exist"**: Run `dotnet tool install --global dotnet-ef` first
- **Database connection errors**: Ensure PostgreSQL container is running with `docker ps`
- **Port 5041 already in use**: Stop any existing dotnet processes with `pkill dotnet`

### No Linting/Formatting Tools
- No EditorConfig, ESLint, or other formatting tools are configured
- No pre-commit hooks or code style enforcement
- Standard .NET compiler warnings are the only code quality checks

### User Secrets
- Application mentions user secrets in README but runs successfully without them
- User secrets are optional for development (actual secrets needed from maintainer for full functionality)

### Timing Expectations
- Package restore: ~18 seconds (set 30+ second timeout)
- Build: ~15 seconds (set 30+ second timeout)  
- Database setup: ~6 seconds (set 15+ second timeout)
- Database migrations: ~7 seconds (set 15+ second timeout)
- Application startup: ~4 seconds
- No operations require 60+ minute timeouts

### Manual Testing Scenarios
After making changes, ALWAYS test these scenarios:
1. **Homepage Navigation**: Navigate to http://localhost:5041, verify "LiteInvoice" header displays
2. **Registration Flow**: Click "Register", verify form loads with Email/Password/Confirm Password fields
3. **Navigation**: Click "Home" to return, verify no console errors
4. **Database Connectivity**: Check application logs for successful EF migration messages
5. **SignalR Connection**: Verify browser console shows "WebSocket connected" message

### GitHub Workflows
- `.github/workflows/main.yml`: Build and push to GitHub Container Registry
- `.github/workflows/setversion.yml`: Version tagging workflow
- Both use .NET 9.0 and require manual trigger (workflow_dispatch)