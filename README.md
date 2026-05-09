# Northwind Traders Order Management System

Internal order management system for Northwind Traders. This application is designed for employees who need to create, update, validate, track, analyze, and report customer orders using the Northwind SQL Server database.

The project includes a Clean Architecture ASP.NET Core Web API backend, Entity Framework Core Database First persistence, Google Maps address validation workflows, PDF and Excel exports, and a Vue 3 + Quasar + TypeScript frontend.

## Demo Video

Paste the YouTube demo video link here:

```text
https://youtu.be/fJtie-oytns
```

## Technology Stack

- Backend: .NET 10, ASP.NET Core Web API, C#, controllers
- Architecture: Clean Architecture, Repository Pattern, Unit of Work
- Database: SQL Server 2022, Northwind database, Entity Framework Core Database First
- Validation: FluentValidation
- Reporting: QuestPDF and ClosedXML
- Frontend: Vue 3, Quasar, TypeScript, Pinia, Axios, Chart.js
- Testing: xUnit, Moq, FluentAssertions

## Project Structure

```text
NorthwindTraders.sln
src/
  NorthwindTraders.Api/
  NorthwindTraders.Application/
  NorthwindTraders.Domain/
  NorthwindTraders.Infrastructure/
tests/
  NorthwindTraders.UnitTests/
client/
  northwind-traders-ui/
database/
  instnwnd.sql
  create_order_shipping_validations.sql
```

## Prerequisites

Install these tools before running the project:

- .NET 10 SDK
- Node.js 20 or later
- npm
- SQL Server 2022 or SQL Server Developer Edition
- SQL Server Management Studio or Azure Data Studio
- Git
- Google Maps API key, only if you want real address validation

## 1. Clone the Repository

```bash
git clone <repository-url>
cd "RSM FINAL PROJECT"
```

If you already have the repository locally, open a terminal in the project root.

## 2. Create the Database

Open SQL Server Management Studio or Azure Data Studio and run:

```text
database/instnwnd.sql
```

This creates the Northwind database and its sample data.

Then run:

```text
database/create_order_shipping_validations.sql
```

This adds the application-owned table used to store saved Google address validation results. It does not change the original Northwind order tables.

## 3. Configure the Backend

The backend expects a connection string named `NorthwindDatabase`.

Recommended option using .NET user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:NorthwindDatabase" "Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;" --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj
```

Alternative option using a temporary PowerShell environment variable:

```powershell
$env:ConnectionStrings__NorthwindDatabase="Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
```

For SQL Server authentication, use a connection string similar to:

```text
Server=localhost;Database=Northwind;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;
```

Do not commit real passwords or connection strings.

## 4. Configure Google Maps Address Validation

The project can run without a Google Maps API key, but real address validation requires one.

Set the API key with user secrets:

```bash
dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_MAPS_API_KEY" --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj
```

Or use a temporary PowerShell environment variable:

```powershell
$env:GoogleMaps__ApiKey="YOUR_GOOGLE_MAPS_API_KEY"
```

The default API URLs are configured in `src/NorthwindTraders.Api/appsettings.json`:

```json
{
  "GoogleMaps": {
    "AddressValidationBaseUrl": "https://addressvalidation.googleapis.com",
    "GeocodingBaseUrl": "https://maps.googleapis.com"
  }
}
```

Do not commit real API keys.

## 5. Restore and Build the Backend

From the repository root:

```bash
dotnet restore
dotnet build
```

## 6. Run the Backend API

From the repository root:

```bash
dotnet run --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj --launch-profile http
```

The API runs at:

```text
http://localhost:5083
```

Swagger is available in Development at:

```text
http://localhost:5083/swagger
```

## 7. Install and Run the Frontend

Open a second terminal and run:

```bash
cd client/northwind-traders-ui
npm install
npm run dev
```

The Quasar dev server runs at:

```text
http://localhost:9000
```

In development, the frontend API base URL is configured as:

```text
http://localhost:5083
```

## Useful Commands

Backend:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj --launch-profile http
```

Frontend:

```bash
cd client/northwind-traders-ui
npm install
npm run dev
npm run build
npm run lint
npm run typecheck
```

## Main Features

- Order create, read, update, and delete workflows
- Customer, employee, shipper, and product lookup endpoints
- Google Maps address validation
- Saved validated shipping addresses and coordinates
- Embedded map preview for validated shipment locations
- Individual order PDF export
- Filtered reporting dashboard
- Excel and PDF report exports

## API Smoke Test

After starting the backend, you can test address validation with:

```http
POST http://localhost:5083/api/address-validation/validate
Content-Type: application/json
```

Sample body:

```json
{
  "addressLine": "1600 Amphitheatre Pkwy",
  "city": "Mountain View",
  "region": "CA",
  "postalCode": "94043",
  "country": "US"
}
```

## Troubleshooting

- If the backend fails on startup, verify that `ConnectionStrings:NorthwindDatabase` is configured and SQL Server is running.
- If order detail coordinates are missing, confirm that `database/create_order_shipping_validations.sql` was executed.
- If address validation returns unavailable or unauthorized responses, verify that the Google Maps API key is configured, billing is active, and the Address Validation API is enabled.
- If the frontend cannot reach the API, confirm that the backend is running at `http://localhost:5083`.
- If npm commands fail, confirm that Node.js 20 or later is installed.

## Notes

- Authentication is not implemented by design.
- Docker support is treated as a final bonus and is not required to run the project locally.
- Keep code, file names, comments, UI labels, and documentation in English.
