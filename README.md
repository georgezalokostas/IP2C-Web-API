# IP2C-Web-API

A .NET Core WEB API where users can get the country of a given IP.

# Installation

These dependencies should be installed as well:
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Dapper
dotnet add package RestSharp
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Toolkit
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer 
dotnet add package Swashbuckle.AspNetCore.Filters
```

Also we can get the Seed Data from the data_seed.txt file, and then scaffold the DB like this:
```
dotnet ef dbcontext scaffold Name=DefaultConnection  Microsoft.EntityFrameworkCore.SqlServer -o Models
```

# Endpoints

# Usage

This project 

# License

This project is licensed under the MIT License. See the LICENSE file for more information.
