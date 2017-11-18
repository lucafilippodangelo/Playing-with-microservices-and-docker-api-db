# Docker-Hello-World
Creation of two containers, mvc app and database. Then communication between them.

DESCRIPTION:  
- simple mvc app with separated SQL container
  - it does just a read from a seeded database.

How to connect to the DB in container bfrom MSSQL Management
  - with this connection string:
    Server=tcp:127.0.0.1,5433;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=Pass@word
  - This is how to connect from management studio
    127.0.0.1,5433
    sa
    Pass@word
  - appsetting.json
    {
	  "ConnectionString": "Server=tcp:127.0.0.1,5433;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=Pass@word",
	  "CatalogBaseUrl": "http://localhost:5106",
	  "Logging": {
	    "IncludeScopes": false,
	    "LogLevel": {
	      "Default": "Warning"
	    }
	  }
	}
  - docker-compose.yml
    version: '3'
	services:
	  eshopweb:
	    image: eshop/web
	    build:
	      context: ./eShopWeb
	      dockerfile: Dockerfile
	      
	    depends_on:
	      - sql.data #depend on the DATABASE service we create
	  sql.data:
	    image: microsoft/mssql-server-linux:latest
	    environment:
	    - SA_PASSWORD=Pass@word
	    - ACCEPT_EULA=Y
	    ports:
	    - "5434:1433"
  - docker-compose.override.yml
    version: '3'
	services:
	  eshopweb:
	    environment:
	      - ASPNETCORE_ENVIRONMENT=Development
	      - ConnectionString=Server=sql.data;Database=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=Pass@word
	      - CatalogBaseUrl=http://localhost:5106
	    ports:
	      - "5106:5106"

	  sql.data:
	    environment:
	      - MSSQL_SA_PASSWORD=Pass@word
	      - ACCEPT_EULA=Y
	    ports:
	      - "5433:1433"
    - docker running container, launched by visual studio
    13049647284c        microsoft/mssql-server-linux:latest   "/bin/sh -c /opt/m..."   14 hours ago        Up 14 hours         0.0.0.0:5433->1433/tcp, 0.0.0.0:5434->1433/tcp   dockercompose12275427739607584068_sql.data_1	  
