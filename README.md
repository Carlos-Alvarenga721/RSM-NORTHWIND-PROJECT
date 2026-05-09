# Northwind Traders Order Management System

Internal order management system for Northwind Traders. The project uses the Northwind SQL Server database to manage orders, customers, employees, shippers, products, reporting, and address validation workflows.

Current implementation includes the Clean Architecture backend, Northwind EF Core Database First persistence, lookup endpoints, Orders CRUD, Google Maps address validation, PDF/Excel exports, and a Vue 3 + Quasar + TypeScript frontend under `client/northwind-traders-ui`.

## Database Setup

1. Create the Northwind database with:

```bash
database/instnwnd.sql
```

2. Add the small application-owned table for saved Google address validation results:

```bash
database/create_order_shipping_validations.sql
```

This table does not change the original Northwind order tables. It only stores the validated address, coordinates, validation status, and Google place id for each order.

Configure the SQL Server connection string through user-secrets or an environment variable. Do not commit real database passwords.

Example environment variable name:

```powershell
$env:ConnectionStrings__NorthwindDatabase="Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Google Maps Address Validation Setup

Configure the Google Maps Address Validation API key using a Windows environment variable. Do not commit real API keys.
The API key is read from `GoogleMaps:ApiKey` (environment variable `GoogleMaps__ApiKey`).
The base URL is read from `GoogleMaps:AddressValidationBaseUrl` and defaults to `https://addressvalidation.googleapis.com`.

PowerShell temporary (current session):

```powershell
$env:GoogleMaps__ApiKey="YOUR_GOOGLE_MAPS_API_KEY"
```

PowerShell persistent user variable:

```powershell
[Environment]::SetEnvironmentVariable("GoogleMaps__ApiKey", "YOUR_GOOGLE_MAPS_API_KEY", "User")
```

Run the backend:

```powershell
dotnet run --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj --launch-profile http
```

Swagger is available at the backend root URL in Development.

Test endpoint:

`POST http://localhost:5083/api/address-validation/validate`

Sample request body:

```json
{
	"addressLine": "1600 Amphitheatre Pkwy",
	"city": "Mountain View",
	"region": "CA",
	"postalCode": "94043",
	"country": "US"
}
```

## Frontend Setup

```bash
cd client/northwind-traders-ui
npm install
npm run dev
```

The Quasar dev server defaults to `http://localhost:9000`.

## Reports and Exports

- Reports page: `/reports`
- Excel export: filtered order report table
- PDF export: filtered order report table
- Individual order PDF: available from the order detail page

## Run Tests and Builds

Backend:

```bash
dotnet restore
dotnet build
dotnet test
```

Frontend:

```bash
cd client/northwind-traders-ui
npm run build
```

## Known Limitations

- Google address validation requires a valid backend API key. Without it, the app uses the controlled `ValidationUnavailable` fallback.
- The map preview uses an embedded Google Maps URL and does not expose a frontend API key.
- Run `database/create_order_shipping_validations.sql` before demoing saved coordinates on order details and PDFs.
