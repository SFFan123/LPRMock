[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

# LPRMock
A LPR Mock server with a REST(like) API to mock LPR server to test printing calls from other progams.

## Requirements
- dotnet 6
- privileged execution to open TCP Listner on Port 515.

## Settings
In the appsettings.json you can define 
- the LPR Port (default 515)
- the LPR IP to bind to (default 127.0.0.1)
- the API Url to bind to
- the Allowed hosts filter for ASP.Net

## Included
- LPR Mock Server
  - Conditional rejection of print jobs controlled by Print Filters over REST API
- REST(like) API to
  - List Printjobs, Purge Print queue.
  - get LPR Endpoint (Bound IP + Port)
  - Get Print Filters (queue names, host names, user names) - Empty allows all
  - Add Print Filters
  - Remove/Purge Print Filters

## Limitations
- Some LPR Commands are ignored.
- No validation from Control file to Data File.

## Used Nugets
- Swashbuckle.AspNetCore - MIT License

  
