I do a little bit of freelance work and need to create invoices. There are invoicing solutions out there, and [Billdu](https://www.billdu.com/) comes closest to what I want. But I can't justify a monthly subscription for the tiny volume I have. I don't want to use Word, Excel or Google Docs. Those approaches are very manual. I've used PayPal invoicing. I didn't really like its item management experience, but tracking and receiving payment is easy. One of my two customers doesn't use PayPal, however, so I need a system that can use PayPal but not depend on it. I tried Square, but I couldn't get past their identity verification step. They didn't believe I was who I said I was.

I'd like a single unified interface for entering hours/expenses and creating invoices that the customer can view without a login, and offer payment links. I'd like to connect or plug in several different payment providers -- PayPal, Stripe, and who knows what else. One customer uses snail mail, so simply having my mailing address on the invoice works.

# Setting up new dev environment
1. Install Docker Desktop if needed: `winget install Docker.DockerDesktop`
2. Setup local Postgres instance with [pg-setup-docker.txt](https://github.com/adamfoneil/LiteInvoice/blob/master/pg-setup-docker.txt)
3. Create local database by `cd LiteInvoice.Database` then `dotnet ef database update`
4. Right click on the Blazor project and select Manage User Secrets. Get the secrets.json file from Adam.
