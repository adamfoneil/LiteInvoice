# Setting up new dev environment
1. Install Docker Desktop if needed: `winget install Docker.DockerDesktop`
2. Setup local Postgres instance with [pg-setup-docker.txt](https://github.com/adamfoneil/LiteInvoice/blob/master/pg-setup-docker.txt)
3. Create local database by `cd LiteInvoice.Database` then `dotnet ef database update`
4. Right click on the Blazor project and select Manage User Secrets. Get the secrets.json file from Adam.
