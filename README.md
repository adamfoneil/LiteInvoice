I do a little bit of freelance work and need to create invoices. There are invoicing solutions out there, and [Billdu](https://www.billdu.com/) comes closest to what I want. But I can't justify a monthly subscription. I've used PayPal invoicing. I'm not wild about its UX, but tracking and receiving payment is easy. One of my two customers doesn't use PayPal, however, so I need a system that can use PayPal but not depend on it. I tried Square, but I couldn't get past their identity verification step. They didn't believe I was who I said I was.

# Setting up new dev environment
1. Install Docker Desktop if needed: `winget install Docker.DockerDesktop`
2. Setup local Postgres instance with [pg-setup-docker.txt](https://github.com/adamfoneil/LiteInvoice/blob/master/pg-setup-docker.txt)
3. Create local database by `cd LiteInvoice.Database` then `dotnet ef database update`
4. Right click on the Blazor project and select Manage User Secrets. Get the secrets.json file from Adam.
