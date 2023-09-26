# SalesRepo

## Description
A solution containing a simple API for managing customers, products and orders.

## Requirements to run

1) .Net 7.0
2) SQL Server
3) Docker

## Projects

### SalesRepo.Api

The main project containing the  API.

### SalesRepo.Data

Contains the data access layer.

### SalesRepo.Domain

Contains the domain logic for the API project

### SalesRepo.IntegrationTests

Contains integration tests

### SalesRepo.UnitTests

Contains unit tests

## Architecture

The project consists of the following components:
1) A RESTful JSON over HTTP API for managing customers, products and orders
2) A backing SQL server store for persisting the details of customers, products and orders

### How to run:

1) Create a local database by running the following commad in the SalesRepo\SaleRepo.Api folder using your choice of shell:

    ```dotnet ef database update```

    This will create a local SQL Server database on your machine.

2) Fromt the same folder (C:\Code\SalesRepo\SalesRepo.Api) run the following commad:
    ```dotnet user-jwts create```
    This will generate a JWT token that can be used to access the authentciated endpoints ```eyJhbGciOiJIUzI1NiIsInR5cC...```

3) Run the API using the dockerfile located at SalesRepo\SalesRepo.Api\Dockerfile:

4) In a browser navigate to the OpenAPI (swagger) page of the API - it should look like this: https://localhost:{port}/swagger/index.html

5) At the top of the page click the "Authorize" button and enter "Bearer {token}" where {token} is the token from step 2).

6) The end points should now all be testable using the "Try it out" buttons.
