# Northwind Traders Project Context Summary

This document is a detailed context summary for the Northwind Traders Order Management System. It is intended to be passed to an AI assistant or developer so they can understand the project structure, architecture, implemented features, technology stack, database model, backend contracts, frontend structure, and current development rules.

## 1. Project Overview

Northwind Traders Order Management System is an internal business application for company employees. It is not a public e-commerce storefront. The system is focused on operational order management over the SQL Server Northwind sample database.

Main business goals:

- Create, read, update, and delete customer orders.
- Select customers, employees, shippers, and products from Northwind lookup data.
- Manage order line items with quantities, unit prices, discounts, freight, and calculated totals.
- Validate shipping addresses through Google Maps Address Validation API.
- Store validated shipping address metadata and coordinates for saved orders.
- Display validated shipment coordinates in the frontend through an embedded map preview.
- Generate individual PDF reports for order details.
- Provide a reporting dashboard with charts and filtered order tables.
- Export filtered report data to Excel and PDF.

Important project rule:

- All source code, file names, folders, classes, variables, methods, comments, UI labels, and documentation must be written in English.

Authentication is intentionally not implemented unless explicitly requested.

## 2. Repository Root Structure

```text
NorthwindTraders.sln
global.json
README.md
AGENTS.md
docker-compose.yml
.env.example
PROJECT_CONTEXT_SUMMARY.md
database/
  instnwnd.sql
  create_order_shipping_validations.sql
docs/
  northwind_traders_vs_code_agent_context.md
src/
  NorthwindTraders.Api/
  NorthwindTraders.Application/
  NorthwindTraders.Domain/
  NorthwindTraders.Infrastructure/
tests/
  NorthwindTraders.UnitTests/
client/
  northwind-traders-ui/
```

Root-level files:

- `NorthwindTraders.sln`: .NET solution containing backend and test projects.
- `global.json`: pins .NET SDK version to `10.0.103`.
- `README.md`: main setup/run/test instructions.
- `AGENTS.md`: repository-specific agent instructions.
- `docker-compose.yml`: optional SQL Server container support.
- `.env.example`: environment placeholder file.
- `database/instnwnd.sql`: Northwind sample database script.
- `database/create_order_shipping_validations.sql`: application-owned table script for validated shipping metadata.

## 3. Technology Stack

Backend:

- .NET 10
- C#
- ASP.NET Core Web API
- Controller-based APIs
- Entity Framework Core 10
- SQL Server provider for EF Core
- FluentValidation
- Repository Pattern
- Unit of Work Pattern
- Swagger/OpenAPI through `Microsoft.AspNetCore.OpenApi` and `Swashbuckle.AspNetCore`
- QuestPDF for PDF generation
- ClosedXML for Excel export
- `HttpClientFactory` for Google Maps API integration

Frontend:

- Vue 3
- Quasar Framework 2
- TypeScript
- Composition API
- Pinia
- Vue Router
- Axios
- Chart.js
- vue-chartjs
- Quasar Notify, Dialog, Loading, Layout, Table, Form, and related components

Database:

- SQL Server 2022 / SQL Server Developer Edition
- Northwind sample database
- EF Core Database First / reverse-engineered persistence model
- Application-owned `OrderShippingValidations` table linked to Northwind `Orders`

Testing:

- xUnit
- Moq
- FluentAssertions
- coverlet collector
- Microsoft.NET.Test.Sdk

Optional infrastructure:

- Docker Compose service for SQL Server 2022

## 4. Architecture

The backend follows Clean Architecture with four main projects:

```text
src/
  NorthwindTraders.Domain/
  NorthwindTraders.Application/
  NorthwindTraders.Infrastructure/
  NorthwindTraders.Api/
```

Dependency direction:

- `Domain` depends on no other project.
- `Application` depends only on `Domain`.
- `Infrastructure` depends on `Application` and `Domain`.
- `Api` depends on `Application` and `Infrastructure`.
- `Tests` reference the projects they test.

Expected request flow:

```text
HTTP Request
  -> Api Controller
  -> Application Use Case
  -> Application Abstractions
  -> Infrastructure Repository or Service
  -> EF Core DbContext / External API / Report Generator
  -> Response DTO
```

Controllers are intentionally thin. They do not access `NorthwindDbContext` directly and do not contain business logic.

## 5. Backend Projects

### 5.1 NorthwindTraders.Domain

Path:

```text
src/NorthwindTraders.Domain/
```

Target framework:

```text
net10.0
```

Current purpose:

- Stores domain-level business constants and rules that must remain independent from ASP.NET Core, EF Core, SQL Server, Google Maps, and frontend concerns.

Current important file:

```text
src/NorthwindTraders.Domain/Orders/OrderBusinessRules.cs
```

Current order rules:

- `CustomerId` max length: 5.
- `ShipName` max length: 40.
- `ShipAddress` max length: 60.
- `ShipCity` max length: 15.
- `ShipRegion` max length: 15.
- `ShipPostalCode` max length: 10.
- `ShipCountry` max length: 15.
- Minimum freight: 0.
- Minimum unit price: 0.
- Minimum quantity: 1.
- Discount range: 0 to 1.

### 5.2 NorthwindTraders.Application

Path:

```text
src/NorthwindTraders.Application/
```

Target framework:

```text
net10.0
```

Project dependencies:

- References `NorthwindTraders.Domain`.
- Uses FluentValidation.

Responsibilities:

- Use cases.
- DTOs.
- Validation.
- Repository and service abstractions.
- Application-level exceptions.
- Dependency injection registration for application services.

Main folders:

```text
Abstractions/
  Persistence/
  Services/
Common/
  Exceptions/
DTOs/
  AddressValidation/
  Customers/
  Employees/
  Orders/
  Products/
  Reports/
  Shippers/
UseCases/
  AddressValidation/
  Customers/
  Employees/
  Orders/
  Products/
  Reports/
  Shippers/
Validators/
DependencyInjection/
```

Persistence abstractions:

- `ICustomerRepository`
- `IEmployeeRepository`
- `IShipperRepository`
- `IProductRepository`
- `IOrderRepository`
- `IReportRepository`
- `IUnitOfWork`
- `CreatedOrderReference`

Service abstractions:

- `IAddressValidationService`
- `IOrderPdfReportService`
- `IOrdersReportExportService`

Important use cases:

- `GetCustomersLookupUseCase`
- `GetEmployeesLookupUseCase`
- `GetShippersLookupUseCase`
- `GetProductsLookupUseCase`
- `GetOrdersUseCase`
- `GetOrderByIdUseCase`
- `CreateOrderUseCase`
- `UpdateOrderUseCase`
- `DeleteOrderUseCase`
- `GenerateOrderPdfUseCase`
- `ValidateAddressUseCase`
- `GetOrdersReportUseCase`
- `GetDashboardReportUseCase`
- `ExportOrdersReportUseCase`

Application exception:

- `NotFoundException`: used for missing orders and mapped to HTTP 404 by the API middleware.

### 5.3 NorthwindTraders.Infrastructure

Path:

```text
src/NorthwindTraders.Infrastructure/
```

Target framework:

```text
net10.0
```

Project dependencies:

- References `NorthwindTraders.Application`.
- References `NorthwindTraders.Domain`.

NuGet packages:

- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Http`
- `QuestPDF`
- `ClosedXML`

Responsibilities:

- EF Core DbContext.
- Reverse-engineered Northwind persistence entities.
- Repository implementations.
- Unit of Work implementation.
- Google Maps Address Validation API integration.
- PDF report generation.
- Excel report generation.
- External service options.
- Dependency injection registration for infrastructure services.

Main folders:

```text
Options/
Persistence/
  DbContext/
  Entities/
  Repositories/
  UnitOfWork/
Services/
  AddressValidation/
  Reports/
DependencyInjection/
```

Important infrastructure classes:

- `NorthwindDbContext`
- `CustomerRepository`
- `EmployeeRepository`
- `ShipperRepository`
- `ProductRepository`
- `OrderRepository`
- `ReportRepository`
- `UnitOfWork`
- `GoogleMapsAddressValidationService`
- `QuestPdfOrderReportService`
- `OrdersReportExportService`
- `GoogleMapsOptions`

### 5.4 NorthwindTraders.Api

Path:

```text
src/NorthwindTraders.Api/
```

Target framework:

```text
net10.0
```

Project dependencies:

- References `NorthwindTraders.Application`.
- References `NorthwindTraders.Infrastructure`.

NuGet packages:

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Microsoft.EntityFrameworkCore.Design`

Responsibilities:

- ASP.NET Core Web API host.
- Controller-based REST endpoints.
- CORS configuration for the Quasar frontend.
- Swagger/OpenAPI in development.
- Global exception handling middleware.
- Application and infrastructure dependency registration.

Important files:

```text
src/NorthwindTraders.Api/Program.cs
src/NorthwindTraders.Api/appsettings.json
src/NorthwindTraders.Api/appsettings.Development.json
src/NorthwindTraders.Api/Middleware/ExceptionHandlingMiddleware.cs
src/NorthwindTraders.Api/Controllers/
```

Configured frontend CORS origins:

- `http://localhost:9000`
- `http://127.0.0.1:9000`
- `http://192.168.56.100:9000`

Swagger behavior:

- Swagger is enabled in Development.
- Development root `/` redirects to `/swagger`.

## 6. Backend API Endpoints

### Orders

Controller:

```text
src/NorthwindTraders.Api/Controllers/OrdersController.cs
```

Endpoints:

```text
GET    /api/orders
GET    /api/orders/{orderId}
POST   /api/orders
PUT    /api/orders/{orderId}
DELETE /api/orders/{orderId}
GET    /api/orders/{orderId}/report/pdf
```

Behavior:

- `GET /api/orders` returns order summaries.
- `GET /api/orders/{orderId}` returns one order with details and shipping validation metadata.
- `POST /api/orders` creates an order and returns HTTP 201 with the created order.
- `PUT /api/orders/{orderId}` updates order header, shipping validation metadata, and detail rows.
- `DELETE /api/orders/{orderId}` removes order details before removing the order.
- `GET /api/orders/{orderId}/report/pdf` returns an individual order PDF file.

### Lookups

Controllers:

```text
CustomersController
EmployeesController
ShippersController
ProductsController
```

Endpoints:

```text
GET /api/customers
GET /api/employees
GET /api/shippers
GET /api/products
```

Purpose:

- Provide dropdown/search data for the frontend order form.

### Address Validation

Controller:

```text
src/NorthwindTraders.Api/Controllers/AddressValidationController.cs
```

Endpoint:

```text
POST /api/address-validation/validate
```

Purpose:

- Validates a shipping address through the application use case and infrastructure Google Maps service.

### Reports

Controller:

```text
src/NorthwindTraders.Api/Controllers/ReportsController.cs
```

Endpoints:

```text
GET /api/reports/dashboard
GET /api/reports/orders
GET /api/reports/orders/export/excel
GET /api/reports/orders/export/pdf
```

Query filters:

- `year`
- `month`
- `week`
- `region`

Purpose:

- Dashboard metrics.
- Orders over time chart data.
- Shipments by region chart data.
- Filtered order table rows.
- Excel and PDF exports.

## 7. Backend DTO Contracts

### Order Requests

`CreateOrderRequest` and `UpdateOrderRequest` include:

- `CustomerId`
- `EmployeeId`
- `OrderDate`
- `RequiredDate`
- `ShippedDate`
- `ShipVia`
- `Freight`
- `ShipName`
- `ShipAddress`
- `ShipCity`
- `ShipRegion`
- `ShipPostalCode`
- `ShipCountry`
- `ShippingValidation`
- `Details`

`OrderDetailRequest` includes:

- `ProductId`
- `UnitPrice`
- `Quantity`
- `Discount`

`OrderShippingValidationRequest` includes:

- `OriginalAddress`
- `FormattedAddress`
- `Latitude`
- `Longitude`
- `ValidationStatus`
- `GooglePlaceId`
- `ValidationMessage`
- `ValidationGranularity`
- `GeocodeGranularity`

### Order Responses

`OrderSummaryResponse` includes:

- Order ID.
- Customer ID and customer name.
- Employee ID and employee name.
- Order, required, and shipped dates.
- Shipper ID and shipper name.
- Freight.
- Ship city, region, and country.
- Detail count.
- Order total.

`OrderResponse` includes all summary information plus:

- Ship name.
- Full shipping address fields.
- Shipping validation response.
- Full order details.

`OrderDetailResponse` includes:

- Product ID.
- Product name.
- Unit price.
- Quantity.
- Discount.
- Line total.

### Lookup Responses

`CustomerLookupResponse`:

- `CustomerId`
- `CompanyName`
- `ContactName`
- `City`
- `Country`

`EmployeeLookupResponse`:

- `EmployeeId`
- `FullName`
- `Title`
- `City`
- `Country`

`ShipperLookupResponse`:

- `ShipperId`
- `CompanyName`
- `Phone`

`ProductLookupResponse`:

- `ProductId`
- `ProductName`
- `UnitPrice`
- `UnitsInStock`
- `Discontinued`
- `CategoryId`
- `CategoryName`

### Address Validation DTOs

`AddressValidationRequest`:

- `AddressLine`
- `City`
- `Region`
- `PostalCode`
- `Country`

`AddressValidationResponse`:

- `OriginalAddress`
- `FormattedAddress`
- `Latitude`
- `Longitude`
- `ValidationStatus`
- `GooglePlaceId`
- `ValidationMessage`
- `ValidationGranularity`
- `GeocodeGranularity`

Supported validation status values in frontend/business flow:

- `Validated`
- `NeedsReview`
- `Invalid`
- `ValidationUnavailable`

Note:

- `OrderShippingValidationRequestValidator` currently allows saved statuses `Validated`, `NeedsReview`, and `ValidationUnavailable`.
- `Invalid` is returned by validation but should block order saving in the frontend.

### Report DTOs

`ReportFilterRequest`:

- `Year`
- `Month`
- `Week`
- `Region`

`OrdersReportResponse`:

- `OrdersOverTime`
- `ShipmentsByRegion`
- `Orders`

`DashboardReportResponse`:

- `OrderCount`
- `RegionCount`
- `TotalSales`
- `OrdersReport`

`OrdersOverTimePoint`:

- `Label`
- `OrderCount`
- `TotalSales`

`ShipmentsByRegionPoint`:

- `Region`
- `ShipmentCount`

`ReportOrderRow`:

- `OrderId`
- `CustomerName`
- `EmployeeName`
- `OrderDate`
- `ShippedDate`
- `ShipRegion`
- `ShipCountry`
- `OrderTotal`

## 8. Backend Validation Rules

Validation is implemented with FluentValidation in the Application layer.

Order request validation:

- `CustomerId` is required.
- `CustomerId` max length is 5.
- `CustomerId` must reference an existing customer.
- `EmployeeId` must be greater than 0.
- `EmployeeId` must reference an existing employee.
- `ShipVia` is optional.
- If provided, `ShipVia` must reference an existing shipper.
- `Freight` must be greater than or equal to 0.
- Ship fields must respect Northwind column lengths.
- `Details` is required.
- `Details` must contain at least one item.
- Duplicate product IDs are not allowed in one order.
- Each detail `ProductId` must reference an existing product.
- `UnitPrice` must be greater than or equal to 0.
- `Quantity` must be at least 1.
- `Discount` must be between 0 and 1.
- Optional shipping validation metadata is validated when present.

Address validation request validation:

- `AddressLine` is required.
- `Country` is required.

Report filter validation:

- `Year` must be between 1900 and 2100.
- `Month` must be between 1 and 12.
- `Week` must be between 1 and 53.
- `Region` max length is 50.

## 9. Backend Error Handling

Global middleware:

```text
src/NorthwindTraders.Api/Middleware/ExceptionHandlingMiddleware.cs
```

Behavior:

- `FluentValidation.ValidationException` returns HTTP 400 with validation problem details.
- `NotFoundException` returns HTTP 404 with problem details.
- Unhandled exceptions return HTTP 500 with a safe error title.
- In Development, the 500 response can include exception detail.
- Unexpected exceptions are logged.

The frontend error handler reads ProblemDetails and validation errors and converts them into user-facing Quasar notifications.

## 10. Persistence and Database

Database:

- SQL Server.
- Northwind sample database.
- Main script: `database/instnwnd.sql`.
- Additional application script: `database/create_order_shipping_validations.sql`.

EF Core:

- Uses Database First / reverse-engineered entities.
- DbContext path:

```text
src/NorthwindTraders.Infrastructure/Persistence/DbContext/NorthwindDbContext.cs
```

Important DbSets:

- `Orders`
- `OrderDetails`
- `Customers`
- `Employees`
- `Shippers`
- `Products`
- `Categories`
- `Suppliers`
- `Regions`
- `Territories`
- Northwind views such as `Invoices`, `OrdersQries`, `OrderSubtotals`, `SalesByCategory`, and others.
- `OrderShippingValidations`

Main Northwind order tables:

- `Orders`
- `Order Details`

Important relationship:

- `Order Details` uses composite key `{ OrderId, ProductId }`.
- `Orders` has relationships to `Customers`, `Employees`, `Shippers`, and `Order Details`.
- `OrderShippingValidations` has a one-to-one relationship with `Orders`.

Application-owned table:

```text
dbo.OrderShippingValidations
```

Columns:

- `OrderID int` primary key.
- `OriginalAddress nvarchar(300)`.
- `FormattedAddress nvarchar(300)`.
- `Latitude float`.
- `Longitude float`.
- `ValidationStatus nvarchar(40)`.
- `GooglePlaceId nvarchar(200)`.
- `ValidationMessage nvarchar(500)`.
- `ValidationGranularity nvarchar(50)`.
- `GeocodeGranularity nvarchar(50)`.
- `ValidatedAtUtc datetime2`.

Foreign key:

- `OrderID` references `dbo.Orders(OrderID)` with cascade delete.

Order persistence details:

- `OrderRepository.AddAsync` creates the `Orders` row and returns `CreatedOrderReference`.
- `CreateOrderUseCase` saves the order header first to get the database-generated `OrderId`.
- After the first save, order details are inserted using the generated `OrderId`.
- `UpdateAsync` updates existing details in place when product IDs remain, removes missing details, and adds new details.
- `DeleteAsync` removes details before removing the order.
- Order totals are calculated as line totals plus freight.

Report persistence details:

- `ReportRepository` loads orders with customer, employee, and details.
- Year, month, and region filters are applied in the EF query.
- Week filtering uses `ISOWeek.GetWeekOfYear` and is applied in memory.
- Orders over time are grouped by year and month.
- Shipments by region are grouped by `ShipRegion`, with empty regions shown as `Unassigned`.

## 11. Google Maps Address Validation

Service:

```text
src/NorthwindTraders.Infrastructure/Services/AddressValidation/GoogleMapsAddressValidationService.cs
```

Options:

```text
src/NorthwindTraders.Infrastructure/Options/GoogleMapsOptions.cs
```

Configuration keys:

```json
{
  "GoogleMaps": {
    "ApiKey": "",
    "AddressValidationBaseUrl": "https://addressvalidation.googleapis.com",
    "GeocodingBaseUrl": "https://maps.googleapis.com"
  }
}
```

Behavior:

- If the API key is missing, the service returns `ValidationUnavailable`.
- If the address is empty, the service returns `Invalid`.
- The main API call uses Google Address Validation API endpoint `/v1:validateAddress`.
- The request maps fields to:
  - `regionCode`
  - `locality`
  - `administrativeArea`
  - `postalCode`
  - `addressLines`
- The response reads:
  - `formattedAddress`
  - geocode location latitude and longitude
  - place ID
  - verdict granularity fields
  - address completeness flags
  - unconfirmed/replaced/inferred/spell-corrected component flags
- A complete premise-level address with coordinates and no uncertain components becomes `Validated`.
- Adjusted, inferred, missing-coordinate, or uncertain results become `NeedsReview`.
- Incomplete or non-premise-level results become `Invalid`.
- Certain Google request failures return `ValidationUnavailable`.
- For some bad request cases, the service attempts a Google Geocoding fallback and can return `NeedsReview` with `GEOCODE_FALLBACK`.

Real Google Maps API calls must not be used in unit tests. Unit tests should mock `IAddressValidationService` or use controlled HTTP handlers.

## 12. Reporting and Exporting

Individual order PDF:

- Use case: `GenerateOrderPdfUseCase`.
- Service: `QuestPdfOrderReportService`.
- Endpoint: `GET /api/orders/{orderId}/report/pdf`.
- File name: `northwind-order-{orderId}.pdf`.

Individual order PDF content:

- Northwind Traders branding.
- Order ID.
- Customer.
- Employee.
- Order, required, and shipped dates.
- Shipper.
- Freight.
- Shipping address.
- Validated address.
- Validation status.
- Coordinates.
- Product line items.
- Quantities.
- Unit prices.
- Discounts.
- Line totals.
- Order total.

Filtered report exports:

- Use case: `ExportOrdersReportUseCase`.
- Service: `OrdersReportExportService`.
- Excel endpoint: `GET /api/reports/orders/export/excel`.
- PDF endpoint: `GET /api/reports/orders/export/pdf`.
- Excel file name: `northwind-orders-report.xlsx`.
- PDF file name: `northwind-orders-report.pdf`.

Excel export:

- Uses ClosedXML.
- Includes report title, filters, and order rows.
- Formats dates and currency.

PDF export:

- Uses QuestPDF.
- Landscape A4 layout.
- Includes filter summary, order count, region count, total sales, and order rows.

## 13. Frontend Structure

Frontend root:

```text
client/northwind-traders-ui/
```

Main files:

```text
package.json
package-lock.json
quasar.config.ts
tsconfig.json
eslint.config.js
index.html
src/
  App.vue
  boot/
  components/
  css/
  layouts/
  pages/
  router/
  services/
  stores/
  types/
  utils/
```

Quasar config:

```text
client/northwind-traders-ui/quasar.config.ts
```

Frontend dev server:

```text
http://localhost:9000
```

Frontend API base URL in development:

```text
http://localhost:5083
```

Quasar build settings:

- Router mode: history.
- Dev server port: 9000.
- Browser target includes modern browsers.
- Node target: node20.
- Quasar plugins: Dialog, Loading, Notify.
- Quasar extras: material icons.

## 14. Frontend Pages and Routes

Router file:

```text
client/northwind-traders-ui/src/router/routes.ts
```

Routes:

```text
/                  -> redirects to /orders
/orders            -> OrdersPage
/orders/new        -> OrderFormPage create mode
/orders/:orderId   -> OrderDetailPage
/orders/:orderId/edit -> OrderFormPage edit mode
/reports           -> ReportsDashboardPage
/:catchAll(.*)*    -> redirects to /orders
```

Layout:

```text
client/northwind-traders-ui/src/layouts/MainLayout.vue
```

Layout features:

- Quasar `q-layout`.
- Header with Northwind Traders title.
- Left drawer navigation.
- Navigation items:
  - Orders.
  - Create Order.
  - Reports.

Pages:

- `OrdersPage.vue`: order list, filters, search, table actions, delete confirmation.
- `OrderFormPage.vue`: create/edit wrapper that loads existing order in edit mode and submits create/update requests.
- `OrderDetailPage.vue`: order metadata, details table, address/map preview, PDF download.
- `ReportsDashboardPage.vue`: report filters, charts, metrics, order table, Excel/PDF export actions.

## 15. Frontend Components

Order components:

```text
client/northwind-traders-ui/src/components/orders/
```

Important components:

- `OrderForm.vue`
- `CustomerSelector.vue`
- `EmployeeSelector.vue`
- `ShipperSelector.vue`
- `ProductLineItemsTable.vue`
- `AddressValidationForm.vue`
- `ValidatedAddressPanel.vue`

Map components:

```text
client/northwind-traders-ui/src/components/maps/GoogleMapPreview.vue
```

Report components:

```text
client/northwind-traders-ui/src/components/reports/
  OrdersOverTimeChart.vue
  ShipmentsByRegionChart.vue
```

Frontend behavior:

- Lookup data is loaded into Pinia and reused by selectors.
- Customer selector supports searching by customer fields.
- Employee and shipper selectors use explicit Quasar input/menu/item selection.
- Product line items table supports adding/removing products and calculating line totals.
- Order form calculates item totals and order total client-side.
- Address validation form normalizes common country names/codes and calls backend validation.
- Validated address panel displays validation status, formatted address, coordinates, and messages.
- Google map preview uses validated coordinates or address data to show a map preview.
- Reports charts use Chart.js through vue-chartjs.

## 16. Frontend State Management

Pinia stores:

```text
client/northwind-traders-ui/src/stores/orderStore.ts
client/northwind-traders-ui/src/stores/lookupStore.ts
client/northwind-traders-ui/src/stores/reportStore.ts
```

`orderStore`:

- Stores `orders`.
- Stores `selectedOrder`.
- Tracks `isLoading`.
- Actions:
  - `loadOrders`
  - `loadOrder`
  - `create`
  - `update`
  - `remove`

`lookupStore`:

- Stores customers, employees, shippers, and products.
- Tracks `isLoading` and `hasLoaded`.
- Loads all lookups in parallel through `Promise.all`.

`reportStore`:

- Stores active filters.
- Stores normalized report data.
- Tracks loading and error state.
- Guards against stale concurrent report loads using an active request counter.

## 17. Frontend API Services

Services folder:

```text
client/northwind-traders-ui/src/services/
```

API client:

```text
apiClient.ts
```

Behavior:

- Uses Axios.
- Uses `process.env.API_BASE_URL`.
- Defaults to `https://localhost:7001` if no environment value is injected.
- Quasar dev config injects `http://localhost:5083`.

Service modules:

- `orderService.ts`
- `lookupService.ts`
- `addressValidationService.ts`
- `reportService.ts`
- `errorHandler.ts`

`orderService` endpoints:

- `GET /api/orders`
- `GET /api/orders/{orderId}`
- `POST /api/orders`
- `PUT /api/orders/{orderId}`
- `DELETE /api/orders/{orderId}`
- `GET /api/orders/{orderId}/report/pdf`

`lookupService` endpoints:

- `GET /api/customers`
- `GET /api/employees`
- `GET /api/shippers`
- `GET /api/products`

`addressValidationService` endpoint:

- `POST /api/address-validation/validate`

`reportService` endpoints:

- `GET /api/reports/orders`
- `GET /api/reports/orders/export/excel`
- `GET /api/reports/orders/export/pdf`

Error handling:

- `errorHandler.ts` parses Axios errors.
- It supports API ProblemDetails and validation errors.
- It shows Quasar negative notifications through `notifyApiError`.

File downloads:

```text
client/northwind-traders-ui/src/utils/downloadFile.ts
```

This creates an object URL and programmatically clicks an anchor to download PDF/Excel blobs.

## 18. Frontend TypeScript Contracts

Types folder:

```text
client/northwind-traders-ui/src/types/
```

Important type files:

- `orders.ts`
- `addressValidation.ts`
- `reports.ts`
- `lookups.ts`
- `api.ts`

These mirror backend DTOs closely:

- `CreateOrderRequest`
- `UpdateOrderRequest`
- `OrderResponse`
- `OrderSummaryResponse`
- `OrderDetailRequest`
- `OrderDetailResponse`
- `AddressValidationRequest`
- `AddressValidationResponse`
- `ReportFilterRequest`
- `OrdersReportResponse`
- `ReportOrderRow`
- lookup response types
- ProblemDetails-style API error types

## 19. Core Workflows

### Create Order

1. User opens `/orders/new`.
2. Frontend loads customers, employees, shippers, and products.
3. User selects customer, employee, optional shipper, dates, freight, and shipping address.
4. User adds one or more product line items.
5. Frontend calculates line totals and order total.
6. User validates shipping address.
7. Frontend calls `POST /api/address-validation/validate`.
8. Backend validates the request and calls Google Maps service if configured.
9. Frontend receives validation metadata and displays formatted address/status/map data.
10. User submits order.
11. Frontend calls `POST /api/orders`.
12. Backend validates request, creates order header, saves it, inserts details, saves again, and returns the created order.
13. Frontend navigates to order detail.

### Update Order

1. User opens `/orders/{orderId}/edit`.
2. Frontend loads existing order.
3. User edits order fields, address validation, or line items.
4. Frontend calls `PUT /api/orders/{orderId}`.
5. Backend checks existence, validates request, updates header, shipping validation, and details.
6. Frontend refreshes/navigates with success feedback.

### Delete Order

1. User clicks delete from the orders table.
2. Quasar Dialog asks for confirmation.
3. Frontend calls `DELETE /api/orders/{orderId}`.
4. Backend checks existence, removes details, removes order, and saves.
5. Frontend removes the order from state.

### View Order Details and PDF

1. User opens `/orders/{orderId}`.
2. Frontend calls `GET /api/orders/{orderId}`.
3. Page displays order information, line items, shipping validation, coordinates, and map preview.
4. User can download PDF through `GET /api/orders/{orderId}/report/pdf`.

### Reports Dashboard

1. User opens `/reports`.
2. Frontend loads report data.
3. User can filter by year, month, ISO week, and region.
4. Frontend calls `GET /api/reports/orders` with query params.
5. Charts and order table update.
6. User can export the active filtered result to Excel or PDF.

## 20. Testing

Test project:

```text
tests/NorthwindTraders.UnitTests/
```

Target framework:

```text
net10.0
```

Test packages:

- xUnit
- xUnit runner for Visual Studio
- Moq
- FluentAssertions
- coverlet collector
- Microsoft.NET.Test.Sdk

Test areas currently present:

- Architecture/project setup.
- API controller tests:
  - Orders.
  - Customers.
  - Employees.
  - Shippers.
  - Products.
  - Reports.
  - Address validation.
- Application use case tests:
  - Create order.
  - Delete order.
  - Get orders report.
  - Export orders report.
- Validator tests:
  - Report filters.
  - Order shipping validation request.
- Infrastructure tests:
  - Google Maps address validation service behavior.

Repository notes:

- Unit tests must not call real Google Maps APIs.
- Unit tests should not require a real SQL Server database.
- External dependencies should be mocked or controlled.

Recent documented validation status in project context:

- `dotnet build` passed.
- `dotnet test` passed.
- `npm run typecheck` passed.
- `npm run build` passed.

When continuing development, re-run verification from a local environment with .NET SDK and Node installed.

## 21. Configuration and Secrets

Backend configuration:

```text
src/NorthwindTraders.Api/appsettings.json
src/NorthwindTraders.Api/appsettings.Development.json
```

Required backend connection string:

```text
ConnectionStrings:NorthwindDatabase
```

Recommended local setup with user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:NorthwindDatabase" "Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;" --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj
```

Google Maps API key:

```bash
dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_MAPS_API_KEY" --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj
```

Rules:

- Do not commit real API keys.
- Do not commit real passwords.
- Use user secrets or environment variables for sensitive local configuration.

Frontend development API base URL:

```text
http://localhost:5083
```

Frontend server:

```text
http://localhost:9000
```

## 22. Running the Project

Database setup:

1. Run `database/instnwnd.sql` in SQL Server.
2. Run `database/create_order_shipping_validations.sql`.

Backend:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/NorthwindTraders.Api/NorthwindTraders.Api.csproj --launch-profile http
```

Expected backend URL:

```text
http://localhost:5083
```

Swagger in Development:

```text
http://localhost:5083/swagger
```

Frontend:

```bash
cd client/northwind-traders-ui
npm install
npm run dev
```

Expected frontend URL:

```text
http://localhost:9000
```

Frontend checks:

```bash
npm run typecheck
npm run build
npm run lint
```

## 23. Docker Support

Docker Compose file:

```text
docker-compose.yml
```

Current service:

- `sqlserver`

Image:

```text
mcr.microsoft.com/mssql/server:2022-latest
```

Port mapping:

```text
1433:1433
```

Environment:

- `ACCEPT_EULA=Y`
- `MSSQL_PID=Developer`
- `MSSQL_SA_PASSWORD=${MSSQL_SA_PASSWORD}`

Volume mappings:

- `./database:/scripts:ro`
- `sqlserver-data:/var/opt/mssql`

Docker is optional and treated as a final bonus rather than the primary local development path.

## 24. Important Constraints for Future Work

Do:

- Keep Clean Architecture boundaries intact.
- Keep controllers thin.
- Keep business/application logic in use cases and validators.
- Access SQL Server through repositories and Unit of Work.
- Keep EF Core DbContext in Infrastructure.
- Keep source code and documentation in English.
- Use .NET 10 and `net10.0`.
- Use Vue 3 + Quasar + TypeScript for frontend work.
- Run backend build after structural/backend changes.
- Run tests when test projects or tested behavior are affected.

Do not:

- Do not implement authentication unless explicitly requested.
- Do not access `NorthwindDbContext` from controllers.
- Do not put business logic in controllers.
- Do not hardcode connection strings, passwords, or API keys.
- Do not call real Google Maps APIs from unit tests.
- Do not use Spanish names in code, files, UI labels, comments, or docs.
- Do not add unrelated features.
- Do not modify original Northwind schema unless explicitly approved. The existing added table `OrderShippingValidations` is application-owned and separate from original Northwind order tables.

## 25. Current Pending Work Mentioned in Project Context

The project context document lists these high-priority pending items:

1. Add remaining backend tests for address validation use case, PDF/export services, and additional controller status codes.
2. Update README with setup/run/test/API-key/demo instructions.
3. Prepare final cleanup and demo script.

The root `README.md` already contains substantial setup instructions, but future work should verify it remains accurate against the current implementation.

## 26. Quick Mental Model for Another AI Assistant

This is a Clean Architecture .NET 10 + Vue/Quasar internal operations app over Northwind SQL Server data.

The backend exposes REST controllers. Controllers call Application use cases. Use cases validate with FluentValidation and depend on repository/service interfaces. Infrastructure implements those interfaces using EF Core, Google Maps HTTP calls, QuestPDF, and ClosedXML. The API project wires everything together, configures CORS and Swagger, and uses global exception middleware.

The frontend is a Quasar SPA. It has pages for orders, order form, order details, and reports. It uses Pinia for orders, lookups, and reports. API calls are centralized in typed service modules. Order creation/editing includes address validation and product line management. Reports include charts and export buttons.

The database is mostly the original Northwind schema, plus one application-owned table for saved shipping validation metadata and coordinates. No authentication exists by design.
