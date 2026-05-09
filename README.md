# Northwind Traders Order Management System

Internal order management system for Northwind Traders. The project will use the Northwind SQL Server database to manage orders, customers, employees, shippers, products, reporting, and address validation workflows.

Current implementation includes the Clean Architecture backend, Northwind EF Core Database First persistence, lookup endpoints, Orders CRUD backend, and the initial Vue 3 + Quasar + TypeScript frontend scaffold under `client/northwind-traders-ui`.

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
