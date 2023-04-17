# IP2C-Web-API

This project is a RESTful API for storing and retrieving IP addresses and their associated details, such as country name and codes. The API provides endpoints for user authentication and authorization, as well as querying the IP database for information.
# Installation

These dependencies should be installed as well:
```
dotnet add package Dapper
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer 
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Json
dotnet add package Microsoft.Extensions.Abstractions
dotnet add package Microsoft.Toolkit
dotnet add package RestSharp
dotnet add package StackExchange.Redis
dotnet add package Swashbuckle.AspNetCore.Filters
dotnet tool install --global dotnet-ef
```

Also we can populate the database with some initial seed data from the data_seed.txt file, and then scaffold the DB like this:
```
dotnet ef dbcontext scaffold Name=DefaultConnection  Microsoft.EntityFrameworkCore.SqlServer -o Models
```

On a mac, we should run these 2 commands as well:
```
brew install redis
brew services start redis
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
This API uses Redis as a caching mechanism to improve performance and reduce the response time for frequently requested data. When a request is made for a specific IP address, the API first checks if the data is already stored in the Redis cache. If it is found in the cache, the API returns the cached data to the user without querying the database, which significantly reduces the response time.

If the data is not found in the cache, the API queries the database for the requested data and stores it in Redis with a TTL (Time To Live) of 20 minutes. This means that the data will be available in the cache for 20 minutes and will expire after that period. After expiration, the cache will automatically remove the data, and subsequent requests for the same IP address will trigger a new database query to fetch the latest information.

# Background Sync

The API includes a background sync job that runs every hour to fetch all the IPs from the database in batches of 100 and updates the tables if anything has changed. The batching process is done with pagination, where an infinite loop initially fetches the first 100 data and then skips X times the data it has fetched until no more entries are found.

Each batch is processed by calling the IP2C API for each item. If the country, TwoLetterCode, or ThreeLetterCode has changed, the database is updated. Finally, the cache is also updated, and the new requests will have the updated data.

This background sync feature ensures that the data in the database stays up-to-date and provides accurate information to users who query the API.

# Usage
To use the API, send HTTP requests to the appropriate endpoint using a tool such as cURL or Postman. Be sure to include the required authentication token with each request after logging in.

For more information on the API's endpoints and functionality, see the documentation within the code.

# License

This project is licensed under the MIT License. See the LICENSE file for more information.
