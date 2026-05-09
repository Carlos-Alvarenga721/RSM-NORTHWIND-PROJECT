# Northwind Traders Order Management System
# Technical Context for VS Code AI Agent

## 1. Project Purpose

Build an internal Order Management System for Northwind Traders.

This is not an e-commerce website for final customers. It is an internal business application used by company employees to create, update, validate, track, analyze, and report customer orders.

The application must allow employees to:
- Create, read, update, and delete orders.
- Select customers, employees, shippers, products, and quantities.
- Validate shipping addresses using Google Maps Address Validation API.
- Display validated addresses with latitude and longitude.
- Show validated shipment locations on an embedded Google Map.
- Generate branded PDF reports for individual order details.
- Display a reporting dashboard with charts and an order details table.
- Filter reports by year, month, week, and region.
- Export filtered report data to Excel and PDF.
- Keep the codebase clean, testable, maintainable, and aligned with Clean Architecture.

All source code, variables, classes, methods, files, folders, API routes, database-related names created by the application, comments, documentation, commit messages, and UI labels should be written in English.

---

## 2. Mandatory Technology Stack

### Frontend
- Vue 3.
- Quasar Framework.
- TypeScript.
- Composition API.
- Pinia for state management if global state is needed.
- Axios or a typed API client for HTTP requests.
- Quasar components for forms, tables, notifications, dialogs, layouts, and dashboard UI.
- A chart library compatible with Vue 3 for the dashboard.
- Google Maps JavaScript integration for embedded maps.

### Backend
- C#.
- ASP.NET Core Web API.
- Controller-based REST API.
- Clean Architecture.
- Entity Framework Core.
- FluentValidation.
- Repository Pattern.
- Unit of Work Pattern.
- Global exception handling middleware.
- Swagger/OpenAPI for API documentation.

### Database
- SQL Server Managment 2022.
- Northwind sample database.
- Use the provided `instnwnd.sql` script to have context about the DB
- Use Entity Framework Core Database First / reverse engineering.
- Main Northwind tables:
  - Orders.
  - Order Details.
  - Customers.
  - Employees.
  - Shippers.
  - Products.
  - Categories.
  - Suppliers.

### External APIs
- Google Maps Address Validation API.
- Google Maps geocode result from the validation response.
- Google Maps embedded map or Maps JavaScript API.

### Reporting
- QuestPDF for branded PDF generation.
- ClosedXML for Excel export.
- PDF export for individual order details.
- Excel export for filtered report tables.
- Optional PDF export for filtered report tables.

### Testing
- xUnit.
- Moq.
- FluentAssertions.
- Backend unit tests for controllers, application services, validators, and business logic.
- Target: approximately 80% coverage for services/controllers.
- Frontend tests are bonus only.

### Bonus
- Docker support at the end of the project.
- Frontend-to-backend error handling.
- Frontend tests for form validation and map/address workflows.

---

## 3. Explicit Technical Decisions

Use these decisions unless the user explicitly changes them:

- Use TypeScript, not plain JavaScript.
- Use ASP.NET Core Web API with C#.
- Use controller-based APIs, not Minimal APIs.
- Use Clean Architecture split into separate projects.
- Use SQL Server and the Northwind database.
- Use Entity Framework Core Database First.
- Use Repository + Unit of Work.
- Use FluentValidation for backend request validation.
- Use xUnit + Moq + FluentAssertions for backend tests.
- Do not implement authentication/login unless explicitly requested.
- Treat frontend tests as bonus.
- Treat Docker as final bonus, not first implementation priority.
- Use corporate dashboard UI style similar to the project sample.
- Keep all code and naming in English.

---

## 4. Clean Architecture Solution Structure

Create a backend solution similar to:

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
```

### 4.1 NorthwindTraders.Domain

This project contains enterprise business concepts and rules. It must not depend on ASP.NET Core, Entity Framework Core, SQL Server, Google Maps, Quasar, or any infrastructure framework.

Recommended folders:

```text
NorthwindTraders.Domain/
  Entities/
  ValueObjects/
  Enums/
  Exceptions/
  Common/
```

Recommended domain entities:
- Order.
- OrderDetail.
- Customer.
- Employee.
- Shipper.
- Product.
- Category.
- Supplier.

Recommended value objects:
- ShippingAddress.
- GeoCoordinates.
- Money.
- OrderLineTotal.

Recommended domain rules:
- An order must have a customer.
- An order must have at least one order detail.
- Product quantity must be greater than zero.
- Unit price cannot be negative.
- Freight cannot be negative.
- Shipping address should be validated before final save when address validation is available.
- Order total is calculated from line items plus freight.

Important:
- Keep domain logic independent.
- Do not inject repositories into domain entities.
- Do not place EF Core attributes or DbContext references in the Domain project.

---

### 4.2 NorthwindTraders.Application

This project contains use cases, interfaces, DTOs, validators, commands, queries, and service contracts. It coordinates business workflows but does not know implementation details of SQL Server, Google Maps, PDF, Excel, or HTTP clients.

Recommended folders:

```text
NorthwindTraders.Application/
  Abstractions/
    Persistence/
    Services/
  DTOs/
    Orders/
    Customers/
    Employees/
    Shippers/
    Products/
    Reports/
    AddressValidation/
  UseCases/
    Orders/
    Reports/
    AddressValidation/
  Validators/
  Common/
    Models/
    Exceptions/
    Pagination/
    Results/
```

Recommended interfaces:

```csharp
public interface IOrderRepository { }
public interface ICustomerRepository { }
public interface IEmployeeRepository { }
public interface IShipperRepository { }
public interface IProductRepository { }
public interface IReportRepository { }
public interface IUnitOfWork { }
public interface IAddressValidationService { }
public interface IPdfReportService { }
public interface IExcelExportService { }
```

Recommended order use cases:
- CreateOrder.
- UpdateOrder.
- DeleteOrder.
- GetOrderById.
- GetOrders.
- GetOrderDetails.
- GenerateOrderPdf.
- ValidateOrderShippingAddress.

Recommended reporting use cases:
- GetDashboardMetrics.
- GetOrdersOverTime.
- GetShipmentsByRegion.
- GetFilteredOrderDetails.
- ExportFilteredOrdersToExcel.
- ExportFilteredOrdersToPdf.

Recommended address validation use cases:
- ValidateShippingAddress.
- NormalizeShippingAddress.
- ReturnCoordinatesForValidatedAddress.

Application DTO examples:
- CreateOrderRequest.
- UpdateOrderRequest.
- OrderResponse.
- OrderDetailResponse.
- OrderLineItemRequest.
- AddressValidationRequest.
- AddressValidationResponse.
- DashboardMetricsResponse.
- OrdersOverTimeResponse.
- ShipmentsByRegionResponse.
- ReportFilterRequest.

Important:
- Controllers should call Application services/use cases.
- Application should depend on abstractions, not infrastructure implementations.
- Do not inject DbContext directly into Application services.
- Keep validation in FluentValidation validators.
- Keep mapping clear and explicit.

---

### 4.3 NorthwindTraders.Infrastructure

This project contains external implementations:
- EF Core DbContext.
- SQL Server persistence.
- Repository implementations.
- Unit of Work implementation.
- Google Maps API client.
- QuestPDF implementation.
- ClosedXML implementation.
- Configuration classes.
- External service options.

Recommended folders:

```text
NorthwindTraders.Infrastructure/
  Persistence/
    DbContext/
    Entities/
    Configurations/
    Repositories/
    UnitOfWork/
  ExternalServices/
    GoogleMaps/
  Reports/
    Pdf/
    Excel/
  Options/
  DependencyInjection/
```

Database First guidance:
- Scaffold the existing Northwind database using EF Core reverse engineering.
- Keep generated EF Core persistence models in Infrastructure/Persistence/Entities or Infrastructure/Persistence/Models.
- Keep the DbContext in Infrastructure/Persistence/DbContext.
- Avoid leaking DbContext or EF models directly into API controllers.
- Prefer mapping between EF models and Application/Domain models or DTOs.

Suggested EF Core scaffold command shape:

```bash
dotnet ef dbcontext scaffold "Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context NorthwindDbContext --output-dir Persistence/Entities --context-dir Persistence/DbContext --project src/NorthwindTraders.Infrastructure --startup-project src/NorthwindTraders.Api --force
```

Adjust the connection string according to the local SQL Server environment.

Handling coordinates and validated addresses:
- Northwind Orders has shipping address fields but does not naturally store Google validation metadata such as latitude and longitude.
- Do not modify original Northwind tables unless explicitly approved.
- If persistence of validated coordinates is required, prefer a small additional application-owned table, for example `OrderShippingValidations`, linked to `Orders.OrderID`.
- Store:
  - OrderId.
  - OriginalAddress.
  - FormattedAddress.
  - Latitude.
  - Longitude.
  - ValidationStatus.
  - GooglePlaceId if available.
  - ValidatedAtUtc.
- If avoiding DB changes, coordinates can be calculated and displayed during create/update but will not be available for later tracking without revalidation.

---

### 4.4 NorthwindTraders.Api

This project exposes the REST API. It should be thin. Controllers should not contain business logic.

Recommended folders:

```text
NorthwindTraders.Api/
  Controllers/
  Middleware/
  Extensions/
  Filters/
  Contracts/
  Configuration/
```

Recommended API features:
- Controller-based REST endpoints.
- Swagger/OpenAPI enabled.
- Global exception handling middleware.
- Consistent API response format.
- CORS configured for the Vue/Quasar frontend.
- Environment-based configuration.
- Do not expose Google Maps API keys unnecessarily.
- Do not put business logic in controllers.

Recommended controllers:
- OrdersController.
- CustomersController.
- EmployeesController.
- ShippersController.
- ProductsController.
- ReportsController.
- AddressValidationController.

Recommended endpoints:

```text
GET    /api/orders
GET    /api/orders/{id}
POST   /api/orders
PUT    /api/orders/{id}
DELETE /api/orders/{id}

GET    /api/customers
GET    /api/employees
GET    /api/shippers
GET    /api/products

POST   /api/address-validation/validate

GET    /api/reports/dashboard
GET    /api/reports/orders
GET    /api/reports/orders/export/excel
GET    /api/reports/orders/export/pdf

GET    /api/orders/{id}/report/pdf
```

Recommended API behavior:
- Return 400 for validation errors.
- Return 404 when an order/customer/product does not exist.
- Return 409 for business conflicts when appropriate.
- Return 500 only for unexpected server errors.
- Never return raw exception details to the frontend in production mode.
- Use ProblemDetails or a consistent error response model.

---

## 5. Frontend Structure

Create the Quasar frontend under:

```text
client/northwind-traders-ui/
```

Recommended folders:

```text
src/
  boot/
  components/
    orders/
    reports/
    maps/
    common/
  pages/
    OrdersPage.vue
    OrderFormPage.vue
    OrderDetailPage.vue
    ReportsDashboardPage.vue
  layouts/
    MainLayout.vue
  router/
  stores/
  services/
    apiClient.ts
    orderService.ts
    customerService.ts
    employeeService.ts
    shipperService.ts
    productService.ts
    reportService.ts
    addressValidationService.ts
  types/
    orders.ts
    customers.ts
    employees.ts
    shippers.ts
    products.ts
    reports.ts
    addressValidation.ts
  utils/
```

Recommended pages:
- OrdersPage: list, search, filter, and actions.
- OrderFormPage: create/edit order with customer, products, address validation, and map.
- OrderDetailPage: show order metadata, line items, address, map, and PDF action.
- ReportsDashboardPage: charts, filters, table, Excel/PDF export.

Recommended components:
- OrderForm.
- CustomerSelector.
- EmployeeSelector.
- ShipperSelector.
- ProductLineItemsTable.
- AddressValidationForm.
- ValidatedAddressPanel.
- GoogleMapPreview.
- OrdersTable.
- DashboardFilters.
- OrdersOverTimeChart.
- ShipmentsByRegionChart.
- ReportDetailsTable.
- ExportToolbar.
- AppErrorBanner.
- ConfirmDeleteDialog.

Frontend rules:
- Use TypeScript types for all API requests and responses.
- Do not use `any` unless unavoidable.
- Keep API calls inside services, not directly inside components.
- Use Quasar Notify for user-facing errors/success messages.
- Use Quasar Loading for long-running operations.
- Use reusable components for repeated UI.
- Keep UI labels in English.
- Use a corporate dashboard style.
- Do not implement login unless explicitly requested.

---

## 6. Core Business Workflows

### 6.1 Create Order Workflow

1. Employee opens the order creation page.
2. Frontend loads customers, employees, shippers, and products.
3. Employee selects a customer.
4. Employee selects an order date.
5. Employee assigns an employee responsible for the order.
6. Employee selects shipper.
7. Employee adds one or more product line items.
8. Employee enters quantity for each product.
9. Frontend calculates line subtotals and approximate total for display.
10. Employee enters shipping address.
11. Employee clicks Validate Address.
12. Frontend sends address to backend.
13. Backend calls Google Maps Address Validation API through Infrastructure service.
14. Backend returns standardized address and coordinates.
15. Frontend displays validated address and map marker.
16. Employee reviews all order information.
17. Employee saves the order.
18. Backend validates request with FluentValidation.
19. Application use case applies business rules.
20. Repository persists order using EF Core and SQL Server.
21. API returns created order response.
22. Frontend shows success notification and navigates to order detail page.

### 6.2 Update Order Workflow

1. Employee opens existing order.
2. Frontend loads order details.
3. Employee modifies order fields, line items, or shipping address.
4. If address changes, it must be validated again.
5. Employee saves changes.
6. Backend validates request.
7. Application updates order using repository and Unit of Work.
8. Frontend displays success message.

### 6.3 Delete Order Workflow

1. Employee opens order list or order detail.
2. Employee clicks Delete.
3. Frontend shows confirmation dialog.
4. Employee confirms.
5. Backend verifies order exists.
6. Backend deletes order or applies appropriate deletion strategy.
7. Frontend refreshes list and displays success message.

### 6.4 Generate Individual Order PDF Workflow

1. Employee opens order detail.
2. Employee clicks Generate PDF.
3. Frontend calls `/api/orders/{id}/report/pdf`.
4. Backend loads order, customer, employee, shipper, and line items.
5. Backend uses QuestPDF to generate a branded document.
6. API returns a PDF file stream.
7. Frontend downloads or opens the PDF.

### 6.5 Reporting Dashboard Workflow

1. User opens Reports Dashboard.
2. Frontend loads default metrics.
3. Dashboard displays:
   - Orders over time chart.
   - Shipments by region chart.
   - Order details table.
4. User filters by year, month, week, or region.
5. Frontend calls report endpoints with query parameters.
6. Backend returns filtered metrics and table rows.
7. User exports filtered table to Excel or PDF.
8. Backend generates export file and returns it.

---

## 7. Validation Rules

Use FluentValidation for backend request validation.

Recommended validators:
- CreateOrderRequestValidator.
- UpdateOrderRequestValidator.
- OrderLineItemRequestValidator.
- AddressValidationRequestValidator.
- ReportFilterRequestValidator.

CreateOrderRequest validation:
- CustomerId is required.
- EmployeeId is required.
- ShipperId is required when shipping is selected.
- OrderDate is required.
- At least one line item is required.
- Each line item must have ProductId.
- Each line item quantity must be greater than zero.
- Freight must be zero or greater.
- Shipping address fields must be present before address validation.
- Validated address data should be present before final save when using the validation workflow.

AddressValidationRequest validation:
- AddressLine is required.
- City is recommended.
- Country is required.
- PostalCode is optional depending on country.
- Region is optional depending on country.

ReportFilterRequest validation:
- Year must be valid if provided.
- Month must be between 1 and 12 if provided.
- Week must be valid if provided.
- Region must be sanitized and length-limited.

---

## 8. Error Handling

Backend:
- Implement global exception handling middleware.
- Convert validation exceptions to 400.
- Convert not-found exceptions to 404.
- Convert business rule exceptions to 409 or 400 depending on context.
- Log unexpected exceptions.
- Return safe error messages.

Frontend:
- Centralize HTTP error handling in API client.
- Show validation errors near fields when possible.
- Show global errors using Quasar Notify.
- Do not show raw technical stack traces to users.
- For Google Maps failures, show a clear message: "The address could not be validated. Please review the shipping information and try again."

---

## 9. PDF and Excel Recommendation

### PDF
Use QuestPDF for generating individual order detail reports because it allows PDF documents to be created in C# with a clean fluent API.

PDF report should include:
- Northwind Traders title/branding.
- Order ID.
- Customer.
- Employee.
- Shipper.
- Order date.
- Required date if available.
- Shipped date if available.
- Freight.
- Original shipping address.
- Validated shipping address.
- Latitude and longitude if available.
- Product line items.
- Quantity.
- Unit price.
- Discount if applicable.
- Line total.
- Order total.

### Excel
Use ClosedXML for Excel export because it works well for creating `.xlsx` files in .NET without requiring Microsoft Excel on the server.

Excel export should include:
- Filter metadata.
- Customer.
- Order date.
- Products summary.
- Region.
- Ship country.
- Freight.
- Total.
- Shipper.
- Validated address status if available.

---

## 10. Testing Strategy

Use xUnit + Moq + FluentAssertions.

Recommended test project:

```text
tests/
  NorthwindTraders.UnitTests/
```

Recommended test folders:

```text
NorthwindTraders.UnitTests/
  Application/
    Orders/
    Reports/
    AddressValidation/
  Domain/
  Api/
    Controllers/
  Validators/
```

Test priorities:
- Order creation use case.
- Order update use case.
- Order deletion use case.
- Order total calculation.
- Address validation use case with mocked Google Maps service.
- Report filtering logic.
- PDF generation service contract.
- Excel export service contract.
- FluentValidation validators.
- Controller status codes.

Do not call real Google Maps API in unit tests. Mock `IAddressValidationService`.

Do not require a real SQL Server database for unit tests. Mock repositories and Unit of Work.

Integration tests are optional.

---

## 11. Configuration

Use `appsettings.json`, `appsettings.Development.json`, and environment variables.

Recommended configuration sections:

```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "GoogleMaps": {
    "ApiKey": "DO_NOT_COMMIT_REAL_KEY",
    "AddressValidationBaseUrl": "https://addressvalidation.googleapis.com"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:9000",
      "http://localhost:5173"
    ]
  }
}
```

Rules:
- Never commit real Google Maps API keys.
- Use user secrets or environment variables for local secrets.
- Use configuration binding for typed options.
- Keep API keys out of frontend when validation can be done through backend.
- If the frontend requires Maps JavaScript API key for map rendering, restrict the key by HTTP referrer in Google Cloud Console.

---

## 12. Do Not Use

Do not use these unless the user explicitly asks:
- Plain JavaScript for frontend code.
- Minimal APIs.
- Code First as the primary database approach.
- Direct DbContext access from controllers.
- Business logic inside controllers.
- Authentication/login.
- Real Google Maps API calls inside unit tests.
- Hardcoded API keys.
- Hardcoded connection strings in source code.
- Spanish names for code identifiers.
- `any` in TypeScript unless strictly necessary.
- Overly complex microservices architecture.
- A separate backend per feature.
- Unapproved changes to original Northwind database tables.

---

## 13. Naming Rules

Everything must be written in English.

Examples:
- Use `OrderService`, not `ServicioOrden`.
- Use `CreateOrderRequest`, not `CrearOrdenRequest`.
- Use `ShippingAddress`, not `DireccionEnvio`.
- Use `ValidateAddressAsync`, not `ValidarDireccionAsync`.
- Use `ReportsDashboardPage.vue`, not `PaginaReportes.vue`.
- Use `orders.ts`, not `ordenes.ts`.

Backend naming:
- Classes: PascalCase.
- Methods: PascalCase.
- Async methods end with `Async`.
- Interfaces start with `I`.
- Private fields use `_camelCase`.

Frontend naming:
- Vue components: PascalCase.
- TypeScript interfaces/types: PascalCase.
- Variables/functions: camelCase.
- API services: camelCase file names or clear module names.
- Routes: kebab-case.

Database:
- Keep existing Northwind database names as generated or mapped.
- New application-owned tables should use clear English names.

---

## 14. Official Prompt Implementation Order

Use this prompt order when working with Codex or another AI agent. Complete one prompt, verify that the solution builds, review the generated structure, commit the result, and only then move to the next prompt.

1. Prompt 1: Normalize repository structure.
2. Prompt 2: Create .NET solution and Clean Architecture projects.
3. Prompt 3: Install backend packages and create base folders.
4. Prompt 4: Configure appsettings and prepare EF Core scaffold.
5. Prompt 5: Scaffold Northwind database entities.
6. Prompt 6: Create lookup endpoints.
7. Prompt 7: Create Orders CRUD backend.
8. Prompt 8: Create Quasar TypeScript frontend.
9. Prompt 9: Build Orders UI.
10. Prompt 10: Add Google Maps address validation.
11. Prompt 11: Add reports dashboard.
12. Prompt 12: Add PDF and Excel export.
13. Prompt 13: Add backend tests.
14. Prompt 14: Final README and cleanup.

Important sequencing rule:
- Do not start Orders CRUD before lookup endpoints are finished and manually tested.
- Do not start the frontend before the backend lookup endpoints and Orders CRUD are stable.
- Do not start Google Maps, reports, PDF, or Excel before the core order workflow exists.

---

## 15. Expected Final Deliverables

The final project should include:

- Functional Vue 3 + Quasar frontend.
- ASP.NET Core Web API backend.
- Clean Architecture project structure.
- SQL Server Northwind database integration.
- Order CRUD.
- Address validation with Google Maps.
- Embedded map showing validated address.
- Dashboard with 1-2 charts.
- Order details table with filters.
- Excel export.
- PDF export.
- Individual order PDF report.
- Backend tests with high coverage for services/controllers.
- Setup documentation for:
  - Database.
  - API keys.
  - Backend.
  - Frontend.
  - Testing.
  - Optional Docker.
- Git repository ready for submission.
- English naming and English UI labels.

---

## 16. Agent Behavior Rules

When generating code for this project:

1. Follow the confirmed stack and architecture.
2. Keep code in English.
3. Prefer simple, maintainable code over clever abstractions.
4. Do not introduce unapproved libraries.
5. Do not skip Clean Architecture boundaries.
6. Do not put business logic in controllers or Vue components.
7. Do not expose secrets.
8. Generate tests for backend services/controllers when adding features.
9. Use typed DTOs and TypeScript interfaces.
10. Keep frontend API calls in service modules.
11. Keep backend external integrations behind interfaces.
12. Explain any assumption before applying it if it affects architecture, database schema, or external services.

---

## 17. Current Project Progress

This section summarizes the current implementation state so a new chat or AI agent can continue without losing context.

### 17.1 Current Repository Structure

Current project root:

```text
RSM FINAL PROJECT/
├── .github/copilot-instructions.md
├── database/instnwnd.sql
├── docs/northwind_traders_vs_code_agent_context.md
├── src/
│   ├── NorthwindTraders.Api/
│   ├── NorthwindTraders.Application/
│   ├── NorthwindTraders.Domain/
│   └── NorthwindTraders.Infrastructure/
├── tests/NorthwindTraders.UnitTests/
├── client/
├── docker-compose.yml
├── .env.example
├── .gitignore
├── AGENTS.md
├── global.json
├── NorthwindTraders.sln
└── README.md
```

### 17.2 Completed Work

The following work has already been completed or prepared:

1. Repository structure was normalized for the final project.
2. Clean Architecture backend solution was created.
3. Backend projects were created:
   - `NorthwindTraders.Api`
   - `NorthwindTraders.Application`
   - `NorthwindTraders.Domain`
   - `NorthwindTraders.Infrastructure`
4. Unit test project was created:
   - `NorthwindTraders.UnitTests`
5. Project references were configured between backend projects.
6. Initial backend NuGet packages were installed.
7. Base folders were created inside the backend projects.
8. A placeholder xUnit test was added.
9. `dotnet restore` works.
10. `dotnet build` works.
11. `dotnet test` works with at least one test.
12. SQL Server is running through Docker for local development.
13. `docker-compose.yml` was added only for SQL Server, not full application Dockerization.
14. `.env.example` was added.
15. Local `.env` exists but must not be committed.
16. SQL Server container name:
   - `northwind-sqlserver`
17. Northwind database was created inside the SQL Server container.
18. `instnwnd.sql` was executed successfully.
19. Northwind table counts were validated successfully for key tables such as Customers, Orders, and Products.
20. The API still runs locally for now and connects to SQL Server through `localhost,1433`.
21. When the API is eventually moved into Docker Compose, the SQL Server host must change from `localhost` to the Docker service name, expected to be `sqlserver`.
22. EF Core Database First scaffold was completed from the Dockerized SQL Server Northwind database.
23. Generated EF Core persistence models and `NorthwindDbContext` are located in `NorthwindTraders.Infrastructure`.
24. The scaffold used `--no-onconfiguring` to avoid storing the connection string inside the generated DbContext.
25. Source checks were run to confirm the real local SQL Server password was not committed.
26. Application persistence abstractions were added for lookup/read support and future Orders CRUD.
27. Infrastructure repository implementations were added for lookup/read support and future Orders CRUD.
28. Unit of Work abstraction and implementation were added.
29. Prompt 6 was completed with read-only lookup endpoints for customers, employees, shippers, and products.
30. Application lookup response DTOs and use cases were added for order selector data.
31. Infrastructure repository lookup queries use `AsNoTracking()` and project EF entities into Application response DTOs.
32. Thin API controllers were added for lookup endpoints without injecting `NorthwindDbContext` directly.
33. `dotnet restore`, `dotnet build`, and `dotnet test` were confirmed passing after Prompt 6.
34. Prompt 7 was implemented with complete Orders CRUD backend functionality.
35. `OrdersController` was added with thin REST endpoints for list, detail, create, update, and delete.
36. Orders Application DTOs were added under `NorthwindTraders.Application/DTOs/Orders`.
37. Orders Application use cases were added under `NorthwindTraders.Application/UseCases/Orders`.
38. `IOrderRepository` was expanded to support Orders CRUD operations.
39. `OrderRepository` now maps between Application DTOs and EF Core Northwind persistence entities.
40. Order creation and update use `IUnitOfWork.SaveChangesAsync`.
41. FluentValidation validators were added for create/update order requests and order detail requests.
42. Order business rule constants were added in Domain under `NorthwindTraders.Domain/Orders/OrderBusinessRules.cs`.
43. API exception handling middleware was added to convert FluentValidation errors to 400 and not-found errors to 404.
44. Unit tests were added for Orders controller and selected Orders use cases.
45. Prompt 8 frontend scaffold was created under `client/northwind-traders-ui`.
46. The frontend uses Vue 3, Quasar, TypeScript, Vue Router, Pinia, Axios, Chart.js, and vue-chartjs.
47. Frontend pages were added for orders list, order create/edit form, order detail, and reports dashboard.
48. Frontend reusable components were added for selectors, order form, line items, address validation, validated address display, map preview, and charts.
49. Frontend typed API services and TypeScript API contracts were added for orders, lookups, address validation, reports, PDF, Excel, and error handling.
50. Quasar dev server was confirmed running in the browser, and the Orders list loads live data from the backend.
51. Frontend API base URL was corrected to `http://localhost:5083` for the current local backend.
52. Backend CORS was updated to allow the Quasar development origins currently used locally:
   - `http://localhost:9000`
   - `http://127.0.0.1:9000`
   - `http://192.168.56.100:9000`
53. Lookup selector issues in the order form were fixed by removing the recursive form `v-model` watcher loop and replacing fragile dropdown binding with explicit menu/list selection.
54. Create Order was manually tested through the frontend and now works.
55. Backend order creation was hardened to save the order header first, then insert order details with the generated `OrderId`.
56. The Orders create endpoint now returns `Created($"/api/orders/{order.OrderId}", order)` instead of `CreatedAtAction`, because `CreatedAtAction` was throwing `No route matches the supplied values`.
57. Frontend error handling was improved to avoid showing raw Axios messages such as `Request failed with status code 500`.
58. Prompt 9 Orders UI hardening was completed:
   - Orders list includes pagination, sorting, search, year/month/region filters, ship country filter, and freight display.
   - Orders list includes create, view, edit, and delete actions with confirmation.
   - Order detail shows order information, line items, shipping address, validated address information when available, coordinates, map preview, freight, and order total.
   - Order form supports create/edit, customer/employee/shipper/product selection, product line items, frontend validation, address validation, map preview, and dynamic order total including freight.
59. Prompt 10 address validation backend was implemented:
   - `POST /api/address-validation/validate`.
   - `IAddressValidationService`.
   - `ValidateAddressUseCase`.
   - `GoogleMapsAddressValidationService`.
   - The service reads `GoogleMaps:ApiKey` from configuration and does not hardcode API keys.
   - If no API key is configured, the backend returns `ValidationUnavailable` so local development can continue without a real Google Maps call.
60. Prompt 12 individual order PDF export was partially implemented:
   - `GET /api/orders/{orderId}/report/pdf`.
   - `IOrderPdfReportService`.
   - `GenerateOrderPdfUseCase`.
   - `QuestPdfOrderReportService`.
61. `OrderSummaryResponse` now includes `ShipRegion` so the frontend can filter orders by region.
62. `Microsoft.Extensions.Http` was added to Infrastructure because typed `HttpClient` registration uses `AddHttpClient`.
63. Frontend `tsconfig.json` uses `skipLibCheck: true` to avoid Quasar optional Electron/PWA declaration errors during `vue-tsc`.
64. No EF Core migrations have been created. The project is still using Database First against the existing Northwind schema.
65. No database schema changes were made for address validation coordinates. The current implementation revalidates/display coordinates through the backend service instead of persisting Google validation metadata.

### 17.3 Current Database and Configuration Decisions

Local development connection string shape:

```text
Server=localhost,1433;Database=Northwind;User Id=sa;Password=<local-sa-password>;TrustServerCertificate=True;
```

Rules:
- Do not commit the real local password.
- Do not commit `.env`.
- Use user-secrets for the local API connection string.
- Use `Name=ConnectionStrings:NorthwindDatabase` when scaffolding with EF Core.
- Keep `--no-onconfiguring` in EF Core scaffold commands.
- Keep generated EF Core entities and `NorthwindDbContext` inside `NorthwindTraders.Infrastructure` only.

### 17.4 Current Implementation Boundary

The project has completed the lookup endpoints, Orders CRUD backend, Quasar frontend scaffold, Orders UI hardening, address validation endpoint, and individual order PDF endpoint.

Completed or considered completed:
- Prompt 1: Normalize repository structure.
- Prompt 2: Create .NET solution and Clean Architecture projects.
- Prompt 3: Install backend packages and create base folders.
- Prompt 4: Configure appsettings and prepare EF Core scaffold.
- Prompt 5: Scaffold Northwind database entities.
- Prompt 6: Create lookup endpoints.
- Prompt 7: Create Orders CRUD backend.
- Prompt 8: Create Quasar TypeScript frontend scaffold.
- Prompt 9: Build Orders UI.
- Prompt 10: Add Google Maps address validation.
- Prompt 12 partial: Individual order PDF export through `GET /api/orders/{orderId}/report/pdf`.
- Extra backend preparation: Application persistence abstractions, Infrastructure repositories, Unit of Work base, global exception handling middleware, and focused backend tests.

Next recommended task:
- Prompt 11: Add reports dashboard backend endpoints and repository/query support.

Remaining major tasks:
- Complete Prompt 11 Reports backend:
  - `ReportsController`.
  - `GET /api/reports/dashboard`.
  - `GET /api/reports/orders`.
  - Report DTOs/use cases/repository.
  - Filters by year, month, week, and region.
- Complete Prompt 12 report exports:
  - `GET /api/reports/orders/export/excel` using ClosedXML.
  - Optional `GET /api/reports/orders/export/pdf` or remove/hide the frontend button until implemented.
- Complete Prompt 13 backend tests for validators, report filtering, address validation with mocked service, controllers, and export services.
- Complete Prompt 14 README, setup documentation, known limitations, and demo script.

### 17.5 Completed Task Details: Prompt 6

Prompt 6 must create read-only lookup endpoints required by the future order form:

```text
GET /api/customers
GET /api/employees
GET /api/shippers
GET /api/products
```

Expected flow:

```text
API Controller → Application Use Case → Repository Interface → Infrastructure Repository → NorthwindDbContext → SQL Server
```

Expected Application use cases:
- `GetCustomersLookupUseCase`
- `GetEmployeesLookupUseCase`
- `GetShippersLookupUseCase`
- `GetProductsLookupUseCase`

Expected API controllers:
- `CustomersController`
- `EmployeesController`
- `ShippersController`
- `ProductsController`

Validation completed after Prompt 6:

```bash
dotnet restore
dotnet build
dotnet test
```

Manual API checks for local development:

```text
/api/customers
/api/employees
/api/shippers
/api/products
```

Expected result:
- Each endpoint returns HTTP 200.
- Each endpoint returns JSON arrays from the Northwind database.
- Controllers stay thin.
- No DbContext is injected directly into controllers.
- Orders CRUD is now implemented in Prompt 7.

### 17.6 Completed Task Details: Prompt 7

Prompt 7 implemented the complete backend Orders CRUD workflow.

Implemented API endpoints:

```text
GET    /api/orders
GET    /api/orders/{orderId}
POST   /api/orders
PUT    /api/orders/{orderId}
DELETE /api/orders/{orderId}
```

Expected flow:

```text
OrdersController → Application Use Case → IOrderRepository/IUnitOfWork → OrderRepository → NorthwindDbContext → SQL Server
```

Important files added or updated:

```text
src/NorthwindTraders.Api/Controllers/OrdersController.cs
src/NorthwindTraders.Api/Middleware/ExceptionHandlingMiddleware.cs
src/NorthwindTraders.Api/Program.cs
src/NorthwindTraders.Application/Abstractions/Persistence/IOrderRepository.cs
src/NorthwindTraders.Application/Abstractions/Persistence/CreatedOrderReference.cs
src/NorthwindTraders.Application/Common/Exceptions/NotFoundException.cs
src/NorthwindTraders.Application/DTOs/Orders/
src/NorthwindTraders.Application/UseCases/Orders/
src/NorthwindTraders.Application/DependencyInjection/DependencyInjection.cs
src/NorthwindTraders.Domain/Orders/OrderBusinessRules.cs
src/NorthwindTraders.Infrastructure/Persistence/Repositories/OrderRepository.cs
tests/NorthwindTraders.UnitTests/Api/Controllers/OrdersControllerTests.cs
tests/NorthwindTraders.UnitTests/Application/Orders/
```

Orders DTOs currently include:
- `CreateOrderRequest`
- `UpdateOrderRequest`
- `OrderDetailRequest`
- `OrderResponse`
- `OrderDetailResponse`
- `OrderSummaryResponse`

Orders use cases currently include:
- `GetOrdersUseCase`
- `GetOrderByIdUseCase`
- `CreateOrderUseCase`
- `UpdateOrderUseCase`
- `DeleteOrderUseCase`

Current Orders validation rules:
- `CustomerId` is required, max 5 characters, and must reference an existing customer.
- `EmployeeId` must be greater than zero and must reference an existing employee.
- `ShipVia` is optional, but when provided must reference an existing shipper.
- `Freight` must be zero or greater.
- Ship fields follow Northwind column length limits.
- An order must contain at least one detail.
- An order cannot contain duplicate products.
- Each order detail must reference an existing product.
- `UnitPrice` must be zero or greater.
- `Quantity` must be at least 1.
- `Discount` must be between 0 and 1.

Current API error behavior:
- FluentValidation failures return HTTP 400 with validation problem details.
- `NotFoundException` returns HTTP 404 with problem details.
- Controllers remain thin and do not inject `NorthwindDbContext` directly.

Important implementation notes:
- `OrderRepository.AddAsync` returns `CreatedOrderReference` so the Application layer can read the database-generated `OrderId` after `SaveChangesAsync` without referencing EF entities.
- Order creation currently saves the `Orders` row first, calls `SaveChangesAsync`, then inserts `Order Details` with the generated `OrderId` and calls `SaveChangesAsync` again.
- `OrderRepository.UpdateAsync` updates existing order detail rows in place when product IDs remain, removes missing details, and adds new details. This avoids EF Core tracking conflicts with the Northwind `Order Details` composite key.
- `OrderRepository.DeleteAsync` removes order details before deleting the order.
- Order totals are calculated as line item totals plus freight.
- No database schema changes or migrations were added for Prompt 7.
- `OrdersController.CreateAsync` returns `Created($"/api/orders/{order.OrderId}", order)`. Do not switch it back to `CreatedAtAction` unless route generation is also fixed and tested.

Validation attempted after Prompt 7:

```bash
git diff --check -- <Prompt 7 touched files>
```

Result:
- Passed for files touched by Prompt 7.

Build/test status after Prompt 7:
- `dotnet build` and `dotnet test` were not successfully executed from the current WSL shell.
- `dotnet` was not available on the WSL `PATH`.
- Windows `dotnet.exe` exists at `/mnt/c/Program Files/dotnet/dotnet.exe`, but invoking it from WSL failed before MSBuild started with:

```text
WSL (2) ERROR: UtilBindVsockAnyPort:287: socket failed 1
```

Before moving to Prompt 8, run these commands from an environment where the .NET SDK is available:

```bash
dotnet restore
dotnet build
dotnet test
```

Manual API checks recommended after build:

```text
GET    /api/orders
GET    /api/orders/10248
POST   /api/orders
PUT    /api/orders/{existingOrderId}
DELETE /api/orders/{testOrderId}
```

Use a test-created order for delete checks to avoid removing original Northwind sample data unintentionally.

### 17.7 Completed Task Details: Prompt 8

Prompt 8 created the Quasar TypeScript frontend scaffold.

Frontend root:

```text
client/northwind-traders-ui/
```

Important frontend files added:

```text
client/northwind-traders-ui/package.json
client/northwind-traders-ui/quasar.config.ts
client/northwind-traders-ui/tsconfig.json
client/northwind-traders-ui/src/App.vue
client/northwind-traders-ui/src/boot/axios.ts
client/northwind-traders-ui/src/boot/pinia.ts
client/northwind-traders-ui/src/router/
client/northwind-traders-ui/src/layouts/MainLayout.vue
client/northwind-traders-ui/src/pages/OrdersPage.vue
client/northwind-traders-ui/src/pages/OrderFormPage.vue
client/northwind-traders-ui/src/pages/OrderDetailPage.vue
client/northwind-traders-ui/src/pages/ReportsDashboardPage.vue
client/northwind-traders-ui/src/components/orders/
client/northwind-traders-ui/src/components/maps/
client/northwind-traders-ui/src/components/reports/
client/northwind-traders-ui/src/services/
client/northwind-traders-ui/src/stores/
client/northwind-traders-ui/src/types/
```

Frontend routes:

```text
/orders
/orders/new
/orders/:orderId
/orders/:orderId/edit
/reports
```

Frontend API services currently target:

```text
GET    /api/orders
GET    /api/orders/{orderId}
POST   /api/orders
PUT    /api/orders/{orderId}
DELETE /api/orders/{orderId}
GET    /api/customers
GET    /api/employees
GET    /api/shippers
GET    /api/products
POST   /api/address-validation/validate
GET    /api/reports/orders
GET    /api/reports/orders/export/excel
GET    /api/reports/orders/export/pdf
GET    /api/orders/{orderId}/report/pdf
```

Frontend implementation notes:
- Uses Pinia stores for orders, lookup data, and report data.
- Uses Axios through a typed API client.
- Local development API base URL is currently `http://localhost:5083`.
- Uses Quasar Notify and Loading for user-facing feedback and long-running operations.
- Uses Quasar Dialog for delete confirmation.
- Uses Chart.js and vue-chartjs for dashboard charts.
- Uses Google Maps embed URLs for map preview display.
- Report and Excel UI paths are prepared. Report backend endpoints and report exports still need backend implementation.
- Address validation frontend and backend are now implemented.
- Individual order PDF frontend and backend are now implemented.
- The Customer, Employee, and Shipper selectors currently use explicit `QInput` + `QMenu` + `QItem` selection instead of `QSelect`.
- `OrderForm.vue` intentionally does not emit `update:modelValue` on every internal draft change. A previous deep watcher caused `Maximum recursive updates exceeded` and prevented dropdown selection.
- The Validate Address button calls `POST /api/address-validation/validate`. With `GoogleMaps:ApiKey` configured, the backend calls Google Maps Address Validation API. Without the key, it returns `ValidationUnavailable` for local development.
- Manual smoke test completed:
  - Orders page loads live backend data.
  - Customer and Employee can be selected on Create Order.
  - Product line items can be added.
  - Save Order successfully creates an order through `POST /api/orders`.

Frontend validation attempted after Prompt 8:

```bash
git diff --check -- client/northwind-traders-ui docs/northwind_traders_vs_code_agent_context.md
```

Build/run status after Prompt 8:
- In this assistant shell, `node` was not available and `npm` reported that WSL 1 is unsupported.
- In the user's local environment, `npm install` and `npm run dev` were run successfully.
- Quasar initially required `index.html` to use `<!-- quasar:entry-point -->` instead of `<div id="q-app"></div>`. This was fixed.
- Quasar dev server runs on port `9000`.
- The browser origin may be `http://192.168.56.100:9000`, so that origin is allowed in backend CORS.

Validation status after Prompt 9, Prompt 10, and individual order PDF implementation:

```bash
dotnet build NorthwindTraders.sln
dotnet test NorthwindTraders.sln
cd client/northwind-traders-ui
npm run typecheck
npm run build
```

Result:
- `dotnet build NorthwindTraders.sln` passed with 0 warnings and 0 errors.
- `dotnet test NorthwindTraders.sln` passed with 9 tests.
- `npm run typecheck` passed.
- `npm run build` passed and produced the Quasar SPA under `client/northwind-traders-ui/dist/spa`.

Before continuing frontend work, run from an environment with a working Node.js installation:

```bash
cd client/northwind-traders-ui
npm install
npm run dev
```

### 17.8 Completed Task Details: Prompt 9

Prompt 9 completed the Orders UI page and improved order details.

Implemented frontend behavior:
- Orders table with pagination and sorting.
- Search by order ID, customer, employee, and other visible order metadata.
- Filters by year, month, region, and ship country.
- Columns for order ID, customer, employee, order date, region, ship country, freight, item count, total, and actions.
- Create Order action routes to the create page.
- View Details and Edit actions route to the correct order pages.
- Delete action uses Quasar Dialog confirmation and Notify feedback.
- Order detail page shows order information, product line items, shipping address, map preview, freight, and total.
- Order form requires valid customer, employee, shipping address, validated address, non-duplicate products, valid quantity, valid unit price, valid discount, and non-negative freight.
- Order form calculates order total as line totals plus freight.

Important frontend files:

```text
client/northwind-traders-ui/src/pages/OrdersPage.vue
client/northwind-traders-ui/src/pages/OrderDetailPage.vue
client/northwind-traders-ui/src/pages/OrderFormPage.vue
client/northwind-traders-ui/src/components/orders/OrderForm.vue
client/northwind-traders-ui/src/components/orders/ProductLineItemsTable.vue
client/northwind-traders-ui/src/components/orders/AddressValidationForm.vue
client/northwind-traders-ui/src/components/orders/ValidatedAddressPanel.vue
client/northwind-traders-ui/src/components/maps/GoogleMapPreview.vue
client/northwind-traders-ui/src/stores/orderStore.ts
client/northwind-traders-ui/src/types/orders.ts
```

### 17.9 Completed Task Details: Prompt 10

Prompt 10 added Google Maps address validation support through the backend.

Implemented backend endpoint:

```text
POST /api/address-validation/validate
```

Important backend files:

```text
src/NorthwindTraders.Api/Controllers/AddressValidationController.cs
src/NorthwindTraders.Application/Abstractions/Services/IAddressValidationService.cs
src/NorthwindTraders.Application/DTOs/AddressValidation/AddressValidationRequest.cs
src/NorthwindTraders.Application/DTOs/AddressValidation/AddressValidationResponse.cs
src/NorthwindTraders.Application/UseCases/AddressValidation/ValidateAddress/ValidateAddressUseCase.cs
src/NorthwindTraders.Infrastructure/Services/AddressValidation/GoogleMapsAddressValidationService.cs
```

Important behavior:
- Real API keys must be configured through `GoogleMaps:ApiKey`.
- Real keys must not be committed.
- If `GoogleMaps:ApiKey` is missing, the service returns `ValidationUnavailable`.
- Unit tests must mock `IAddressValidationService` and must not call real Google Maps APIs.

### 17.10 Completed Task Details: Prompt 12 Partial

The individual order PDF export portion of Prompt 12 is implemented.

Implemented backend endpoint:

```text
GET /api/orders/{orderId}/report/pdf
```

Important backend files:

```text
src/NorthwindTraders.Api/Controllers/OrdersController.cs
src/NorthwindTraders.Application/Abstractions/Services/IOrderPdfReportService.cs
src/NorthwindTraders.Application/UseCases/Orders/GenerateOrderPdf/GenerateOrderPdfUseCase.cs
src/NorthwindTraders.Infrastructure/Services/Reports/QuestPdfOrderReportService.cs
```

Still pending from Prompt 12:
- Filtered report Excel export using ClosedXML.
- Optional filtered report PDF export.

### 17.11 Current Pending Work

Highest-priority pending work:

1. Implement Reports backend.
2. Implement filtered report Excel export.
3. Add backend tests for reports, validators, address validation use case, and export services.
4. Update README with setup/run/test/API-key/demo instructions.
5. Decide whether to keep or remove optional report PDF export from the frontend until the backend endpoint exists.

Do not add authentication unless explicitly requested.

### 17.12 Current Git Safety Rules

Before every commit:

```bash
git status
dotnet build
dotnet test
git grep -n "Northwind@2025Dev"
```

Expected:
- Build passes.
- Tests pass.
- `.env` is not staged.
- No real local password appears in tracked files.

Safe placeholder strings such as `Password=DO_NOT_COMMIT_REAL_PASSWORD` are acceptable in documentation, but real local secrets are not.
