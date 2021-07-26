# MerchantTaxApp

A demo solution for various merchants to order furniture, gift, home décor, rug, and apparel items with tax calcualtions based on locality.

# Tech
- NET Core
- Entity Framework Core
- VueJS
- Mediator
- AutoMapper
- FluentValiation
- Xunit, Moq
- TaxJar
- ZipTax (alternative tax calculator)

# Getting started
1. Clone repo
2. Open VS Code terminal
3. Run ``cd src/JavaScriptClient``
4. Run ``.\setupcert.ps1`` to setup localhost cert

# Running Project
## Api
1. Open new VS Code terminal
2. Run ``cd src\Api``
3. Run ``dotnet build``
4. Run ``dotnet run``

## JavaScriptClient
1. Open VS Code terminal 
2. Run ``cd src\JavaScriptClient``
3. Run ``yarn``
4. Run ``yarn serve``
5. Open new VS Code terminal
6. Run ``cd src\JavaScriptClient``
7. Run ``dotnet build``
8. Run ``dotnet run``

Got https://localhost:5001

# Overview

## Domain
Contains all entities specific to the domain with no dependencies on any other projects.

## Application
Contains all application logic and a dependency on the domain layer (has no depenedencies to other layer or project). Interfaces outline
- [ITaxCalculator](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Application/Common/Interfaces/ITaxCalculator.cs)
- [ITaxService](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Application/Common/Interfaces/ITaxService.cs)
- [MerchantService](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Application/Common/Interfaces/IMerchantService.cs)

## Infrastructure
Contains classes for accessing external services as defined by interfaces in the application layer.
- [TaxJarCalculator](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Infrastructure/Services/TaxJarCalculator.cs)
- [TaxService](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Infrastructure/Services/TaxJarCalculator.cs)
- [MerchantService](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Infrastructure/Services/TaxJarCalculator.cs)
- [ZipTaxCalculator](https://github.com/robsmitha/MerchantTaxApp/blob/master/src/Infrastructure/Services/TaxJarCalculator.cs) (alternative tax calculator)

## Api
Contains api endpoints for accessing application layer logic.

## JavaScript Client
Contains a VueJS single page application with .NET Core backend. 