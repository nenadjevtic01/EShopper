# E Shopper

An online clothing store web application, built as a final thesis project. The backend is an ASP.NET Core 6 Web API with Entity Framework Core and a SQL Server database, and the frontend is an Angular 16 application.

## Features

- Shop with filtering by gender, brand, category and size, search, sorting and pagination.
- Product page with an image gallery, size selection and stock tracked per size.
- Cart for visitors who are not logged in, transferred to their account on login.
- Checkout with discount coupons and order confirmation.
- User profile with an avatar, order history and rating of purchased products.
- Promotions with a validity period and product groups with different discount percentages.
- Admin panel: products, catalog, promotions, coupons, orders, users, messages, newsletter, payment methods, hero banner, statistics and an audit log of every action.
- JWT authentication with permissions for each individual operation (use case).

## Technologies

- **Backend:** C#, ASP.NET Core 6 Web API, Entity Framework Core 6, FluentValidation, BCrypt
- **Frontend:** Angular 16, TypeScript, RxJS, SCSS, Bootstrap 4
- **Database:** Microsoft SQL Server

## Structure

```
DiplomskiRadBack/          backend (EShopper.sln solution)
  EShopper.Api             controllers, authentication, configuration
  EShopper.Application     use case interfaces and DTO classes
  EShopper.DataAccess      DbContext, configurations and migrations
  EShopper.Domain          entities
  EShopper.Implementation  commands, queries and validators
DiplomskiRadFront/         frontend (Angular)
DokumentacijaEShopper.pdf  project documentation (in Serbian)
```

The backend and frontend folders must stay next to each other, because the backend saves uploaded images into `DiplomskiRadFront/src/assets/img`.

## Images

Product, category, brand and banner images are not included in this repository, because they are not under a free license. The application works without them, but the shop is shown without pictures. The initial data expects the images in `DiplomskiRadFront/src/assets/img` (one folder per category, plus `brands` and `hero`), and new images can be uploaded through the admin panel.

## Getting started

Requirements: .NET 6 SDK, Node.js 18 or newer and SQL Server (Express is enough).

### 1. Database

The connection string is in `DiplomskiRadBack/EShopper.Api/appsettings.Development.json` (the `ConnectionString` key). By default it points to `localhost\SQLEXPRESS`, database `EShopper`.

The database is created with migrations. In Visual Studio, open the Package Manager Console, set `EShopper.DataAccess` as the Default project and `EShopper.Api` as the startup project, then run:

```
Update-Database
```

### 2. Backend

```
cd DiplomskiRadBack/EShopper.Api
dotnet run
```

The API runs on `http://localhost:5277`, and Swagger is available at `http://localhost:5277/swagger`.

A single call fills the empty database with initial data (users, catalog, products, permissions):

```
POST http://localhost:5277/api/Initial
```

The easiest way is to call it from Swagger. The call is rejected if the database already contains data.

### 3. Frontend

```
cd DiplomskiRadFront
npm install
npm start
```

The application runs on `http://localhost:4200`.

## Test accounts

| Role | Email | Password |
|---|---|---|
| Administrator | admin@gmail.com | Sifra123 |
| Customer | user@gmail.com | Sifra123 |

Sending emails is simulated: messages, including the links for setting and resetting a password, are printed in the backend console.
