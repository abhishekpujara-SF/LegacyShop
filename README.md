# LegacyShop — Modernization Agent test fixture

Small, intentionally legacy ASP.NET app (.NET Framework 4.8, Web Forms + a little MVC 5 / Web API 2, SQL Server). It exists to be modernized by the Modernization Agent, then verified against the baseline below. **Run the agent on a copy** (or a git branch), keeping this folder as the pristine "before".

## Setup

1. Database (SQL Server / LocalDB / Express): `database\setup.cmd [server]` — creates `LegacyShop`, 6 tables, seed data. Login: `admin` / `Passw0rd!`.
2. Edit `Web.config` → `connectionStrings` if your server isn't `.`.
3. Open `LegacyShop.sln` in Visual Studio 2022, restore NuGet, run (IIS Express, port 5801).

## Baseline behaviour (must match after modernization)

| Page / route | Expected |
|---|---|
| `/Default.aspx` | Counts: 6 products, 3 customers, 2 open orders |
| `/Products.aspx` | 6 rows; category filter works; Delete needs login; deleting product 1 fails with message (referenced by orders) |
| `/ProductEdit.aspx` | Requires login (redirects to Login). Add/edit with validation; duplicate SKU shows error |
| `/Customers.aspx` | Grid with inline edit/delete (SqlDataSource), add form; deleting Asha/Ben fails with message |
| `/Orders.aspx` | 3 orders (totals 99.97 / 318.00 / 329.00) with a "Linked" customer count; Items view; "Mark shipped"; New / Edit / Delete (login required to delete; shipped orders cannot be deleted) |
| `/OrderEdit.aspx` (login) | Create/update an order (owning customer, date, status); add/remove items (same product merges, shipped orders are locked) |
| `/OrderCustomers.aspx?orderId=N` (login) | Customers on an order: add an existing customer with a role, change role, remove; owner row is read-only |
| `/CustomerOrders.aspx?customerId=N` (login) | Orders of a customer (owned + linked): add to an existing order with a role, change role, remove link |
| `/Relationships.aspx` | Every customer/order link, filter by customer, role (Owner/Billing/Shipping/Approver/Contact), include/exclude owners |
| `/api/ordersapi`, `/api/ordersapi/N` | JSON orders / one order with its customers |
| `/Login.aspx` | admin / Passw0rd! signs in |
| `/Reports/Index` (MVC) | Low stock (threshold 5): KB-200 (3), MN-270 (2) |
| `/api/productsapi`, `/api/productsapi/1` | JSON list / single product; POST creates; unknown id → 404 |
| `/ExportProducts.ashx` | CSV with 6 data rows |
| Any response | `X-Elapsed-Ms` header (HttpModule) |

## What each modernization area should find

| Area | Where |
|---|---|
| Framework upgrade (net48 → net8+) | non-SDK `.csproj`, `packages.config`, `AssemblyInfo.cs` |
| Web Forms → Razor/MVC | 6 `.aspx` pages + `Site.Master`, GridView/Repeater/validators/SqlDataSource, ViewState-free post-backs |
| NuGet upgrades | Newtonsoft.Json 9.0.1, log4net 2.0.8, Dapper 1.50.2, jQuery 1.12.4, Bootstrap 3.3.7, MVC/WebApi 5.2.3, WebGrease/Antlr (bundling, no .NET Core equivalent) |
| Page/feature inventory | 10 Web Forms pages + Error.aspx (incl. OrderEdit, OrderCustomers, CustomerOrders, Relationships), 1 MVC controller/view, 2 Web API controllers, 1 `.ashx`, 1 HttpModule |
| System.Web / config modernization | `Web.config` (`system.web` auth/session/customErrors/machineKey/httpModules/httpHandlers, `system.webServer`, `location` auth rule, appSettings, binding redirects, log4net section), `Global.asax`, `HttpContext.Current`, `Session`, `FormsAuthentication` |
| MVC / Web API migration | `ReportsController`, `ProductsApiController`, `RouteConfig`, `WebApiConfig`, attribute+convention routes |
| UI / static assets | CDN jQuery/Bootstrap 3 links, `Styles.Render`/`Scripts.Render` bundles, `legacy-ui.css` (vendor prefixes, IE filters, `*zoom`), `site.js`, CSS `url(../Content/img/header-bg.png)` (missing file on purpose) |
| Data access | Dapper (`ProductRepository`), raw ADO.NET `System.Data.SqlClient` incl. transactions and a UNION query (`OrderRepository`, `OrderCustomerRepository`, `CustomerRepository`, `AuthHelper`), declarative `SqlDataSource` |
| Auth/security | Forms auth, unsalted SHA1 password hashes, `AddWithValue` |

## Database

`Categories` 1–* `Products` 1–* `OrderItems` *–1 `Orders` *–1 `Customers`; plus `Users`, and the many-to-many `OrderCustomers(OrderId, CustomerId, Role, AddedOn)` for additional customers on an order (`Orders.CustomerId` remains the owner). Seed: 3 categories, 6 products, 3 customers, 3 orders, 4 order items, 4 order/customer links, 1 user.

Existing databases: run `database\03_order_customers.sql` to add the link table without touching data. Business rules: a customer can be on an order once (never as both owner and participant), changing an order's owner drops that customer's participant row, shipped orders cannot be deleted or have items changed, deleting an order removes its items and links in one transaction.

## Notes

- Authored without a Windows build host: not compiled here. Open in VS 2022 (with the ASP.NET workload) and fix any trivial restore/hint-path drift before using as the "before" baseline.
- The `Content/img/header-bg.png` reference is intentionally dangling for the static-asset check.
