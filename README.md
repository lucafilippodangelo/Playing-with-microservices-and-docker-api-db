# Docker Hello World

Basic project to test containers:

## STEP 1
Creation of two containers, the MVC app and database. 
Implemented communication between them. Silly logic, just read from a seeded database.

## STEP 2
Added the project "crudapi", will be connected with the same sql server in container
  - use of swagger


### How to connect to the DB in container bfrom MSSQL Management

  **with te connection string** -> Server=tcp:127.0.0.1,5433;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=Pass@word

  **and the "docker-compose.override.yml"**

   ```
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
  ```

 **this running container launched by visual studio** 

   ```
   13049647284c        microsoft/mssql-server-linux:latest   "/bin/sh -c /opt/m..."   14 hours ago        Up 14 hours         0.0.0.0:5433->1433/tcp, 0.0.0.0:5434->1433/tcp   dockercompose12275427739607584068_sql.data_1	 
   ```

 **Here the credentials to connect from management studio**

    ```
    127.0.0.1,5433
    sa
    Pass@word 
	 ```

## Useful Links

  - https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker
  - https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker
  - https://hub.docker.com/r/microsoft/mssql-server-linux/