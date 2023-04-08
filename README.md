# IP2C-Web-API

This project is a RESTful API for storing and retrieving IP addresses and their associated details, such as country name and codes. The API provides endpoints for user authentication and authorization, as well as querying the IP database for information.
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

Also we can populate the database with some initial seed data from the data_seed.txt file, and then scaffold the DB like this:
```
dotnet ef dbcontext scaffold Name=DefaultConnection  Microsoft.EntityFrameworkCore.SqlServer -o Models
```
# Endpoints
__Authorization__

In order to access any endpoints, the user must first be authorized by sending a POST request to either /Auth/Register or /Auth/Login with their credentials. Upon successful login, the user will receive a Bearer token which they must provide with each subsequent request.

    POST /Auth/Register - creates a new user account with the provided username and password
    POST /Auth/Login - logs the user in with the provided username and password and returns a Bearer token upon success    

__IP Details__

The GetIPDetails endpoint allows the user to retrieve details about a specific IP address by providing the IP as a parameter in the request.

    GET /api/GetIPDetails/{ip} - returns information about the provided IP, including country name, two-letter code, and three-letter code, if the IP exists in the database

__IP Reports__

The GetReport endpoint allows the user to retrieve a report of the number of IPs stored in the database, grouped by country. Optionally, the user may provide a comma-separated list of two-letter country codes to retrieve data for specific countries.

    GET /api/GetReport - returns a report of the number of IPs stored in the database, grouped by country
    GET /api/GetReport/{codes} - returns a report of the number of IPs stored in the database for the specified countries, as determined by the provided two-letter country codes
    
# Caching and Database Lookup for IP Details in the API
When a user requests IP details from the API, the program first checks if the IP is in the cache. If the IP is found in the cache, it is returned instantly, reducing the need for a database query. If the IP is not in the cache, the program checks the database for the IP. If the IP is found in the database, it is retrieved and updated in the cache for future requests. If the IP is not found in the database, the program fetches the details from the IP2C API, stores it in the cache for future requests, and also stores it in the database to update the IP database. This process ensures that the API provides fast response times and caches commonly requested IPs for improved performance.

# Background Sync

The API includes a background sync job that runs every hour to fetch all the IPs from the database in batches of 100 and updates the tables if anything has changed. The batching process is done with pagination, where an infinite loop initially fetches the first 100 data and then skips X times the data it has fetched until no more entries are found.

Each batch is processed using a Parallel Foreach loop, where each item calls the IP2C API. If the country, TwoLetterCode, or ThreeLetterCode has changed, the database is updated. Finally, the cache is also updated, and the new requests will have the updated data.

This background sync feature ensures that the data in the database stays up-to-date and provides accurate information to users who query the API.

# Usage
To use the API, send HTTP requests to the appropriate endpoint using a tool such as cURL or Postman. Be sure to include the required authentication token with each request after logging in.

For more information on the API's endpoints and functionality, see the documentation within the code.

# License

This project is licensed under the MIT License. See the LICENSE file for more information.
