I do a little bit of freelance work and need to create invoices. There are invoicing solutions out there, and [Billdu](https://www.billdu.com/) comes closest to what I want. But I can't justify a monthly subscription for the tiny volume I have. (Note that Billdu's entry level price has come down to $4.99/month, which is more in line with my expectations.) I don't want to use Word, Excel or Google Docs. Those approaches are too manual for me. I've used PayPal invoicing. I didn't really like its item management experience, but tracking and receiving payment is easy. One of my two customers doesn't use PayPal, however, so I need a system that can use PayPal but not depend on it. I tried Square, but I couldn't get past their identity verification step. They didn't believe I was who I said I was. There is even a [LiteInvoice.com](https://liteinvoice.com/) app that I found after choosing this repo name, so I will likely rename my project. I found their price too high ($29/month starting).

I'd like a single unified interface for entering hours/expenses and creating invoices that the customer can view without a login, and offer payment links. I'd like to connect or plug in several different payment providers -- PayPal, Stripe, and who knows what else. One customer uses snail mail, so simply having my mailing address on the invoice works.

# Architecture
This is a Blazor Server app hosted on DigitalOcean, using a Postgres database. The one other time I used Blazor Server on DO, it didn't work -- for mysterious reasons -- something to do with SignalR. But this time -- again for mysterious reasons -- it works. DigitalOcean offers more competitive pricing than Azure, and Postgres is more than capable for this application. This approach also gives me practice using GitHub workflows and container registry. As of this writing it has the "starter" url. This is fully functional with no paywall, but understand this is work in progress.

[https://liteinvoice-syu54.ondigitalocean.app/](https://liteinvoice-syu54.ondigitalocean.app/)

# Projects
- [LiteInvoice.Database](https://github.com/adamfoneil/LiteInvoice/tree/master/LiteInvoice.Database) -- EF Core DbContext and entities.
- [BlazorApp](https://github.com/adamfoneil/LiteInvoice/tree/master/BlazorApp) -- UI
- [AuthExtensions](https://github.com/adamfoneil/LiteInvoice/tree/master/AuthExtensions) -- a helper for getting the current user in Blazor


# Setting up new dev environment
1. Install Docker Desktop if needed: `winget install Docker.DockerDesktop`
2. Setup local Postgres instance with [pg-setup-docker.txt](https://github.com/adamfoneil/LiteInvoice/blob/master/pg-setup-docker.txt)
3. Create local database by `cd LiteInvoice.Database` then `dotnet ef database update`
4. Right click on the Blazor project and select Manage User Secrets. Get the secrets.json file from Adam.
