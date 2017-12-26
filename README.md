# Docker Hello World

Based on the book [.NET Microservices. Architecture for Containerized .NET Applications](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/), this project is a basic implementation of what learned.

**Useful Links**

  - https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-docker
  - https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker
  - https://hub.docker.com/r/microsoft/mssql-server-linux/

## STEP 1
Creation of two containers, the MVC app and database. 
Implemented communication between them. Silly logic, just read from a seeded database.

## STEP 2
Added the project "crudapi", will be connected with the same sql server in container
  - use of swagger

**How to connect to the DB in container bfrom MSSQL Management**

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

## STEP 3
Personal Notes and reflections.


# BOOK SECTION 4
**"Architecting Container- and Microservice-Based Applications"**


+ pag. 27/297 -> "**Containerizing monolithic applications**"

+ pag. 30/297 ->**Solutions used to manage persistent data in Docker applications**
  - "Data volumes" are directories mapped from the host OS to directories in containers
  - "Data volume containers" are an evolution of regular data volumes. A data volume container is a simple container that has one or more data volumes within it.
  - "Volume plugins" like Flocker provide data access across all hosts in a cluster.
  - "Remote data sources" and cache tools like Azure SQL Database, Azure Document DB, or a remote cache like Redis can be used in containerized applications the same way they are used when developing without containers.

+ pag 32/297 -> **"SOA" Service-oriented architecture**, how to deploy as a docker container. Those services can now be deployed as Docker containers, which solves deployment issues, because all the dependencies are included in the container image. However, when you need to scale up SOA applications, you might have scalability and availability challenges if you are deploying based on single Docker hosts.

+ pag 33/297 -> **Microservices architecture**
   DEFINITION: 
   - As the name implies, a microservices architecture is an approach to building a server application as a set of small services. Each service runs in its own process and communicates with other processes using protocols.

   - Each microservice implements a specific end-to-end domain or business capability within a certain context boundary, and each must be developed autonomously and be deployable independently.

   - Finally, each microservice should own its related domain data model and domain logic (sovereignty and decentralized data management) based on different data storage technologies (SQL, NoSQL) and different programming languages.

  KEY POINTS:
  - create loosely coupled services so you have autonomy of development, deployment, and scale, for each service. 

  - indipendence from other services


+ pag 34/297 -> **Data sovereignty per microservice**
   - each microservice must own its domain data and logic. Just as a full application owns its logic and data, so must each microservice own its logic and data under an autonomous lifecycle, with independent deployment per microservice.

   - here there is a a COMPARISON FOR DATABASES APPROACH

+ pag. 36/297 -> **The relationship between microservices and the Bounded Context pattern**

   - not sharing the model between microservices are what you also want in a Bounded Context.

+ pag. 37/297 -> **Logical architecture versus physical architecture**

   - here is explained the distinction between logical architecture and physical architecture, and how this applies to the design of microservice-based applications

+ pag. 38/297 -> **Challenges and solutions for distributed data management**
 
   - Challenge #1: How to define the boundaries of each microservice
   - Challenge #2: How to create queries that retrieve data from several microservices
     - API Gateway (have an API in the middle that get data from two microservices)
     - CQRS with query/reads tables (materialized view pattern, CQRS approach, here we create a read table to query getting data from two microservices)
     - “Cold data” in central databases (here we have a central database for statistic use with not real time data, usually the data inside the central database are updated by "event-driven communication")
   - Challenge #3: How to achieve consistency across multiple microservices-
     - A microservice cannot directly access a table in another microservice!(example of the Catalog and Basket microservices).
     - CAP Theorem
     - An approach to maintain consistency can be an Event driven communication.

  - Challenge #4: How to design communication across microservice boundaries
    - need to design your microservices and the communication across them taking into account the risks common in this type of distributed system.
    - talks about the risk of synchronous HTTP API calls 

+ pag.42/297 -> **Identifying domain-model boundaries for each microservice**
  - the point is finf the boundaries basing on domain knowledge and not try to reate microservices as small I can.
  - QUESTION: how big a domain model for each microservice should be? it should have an autonomous BC, as isolated as possible, that enables you to work without having to constantly switch to other contexts.
  - Decomposing traditional data models into multiple domain models.
  - Think to a way to MAP AN USER WHEN EXIST IN MULTIPLE DOMAINS
    - Basically, there is a shared concept of a user that exists in multiple services (domains), which all share the identity of that user. But in each domain model there might be additional or different details about the user entity. 
    - There are several benefits to not sharing the same user entity with the same number of attributes across domains. One benefit is to reduce duplication, so that microservice models do not have any data that they do not need. Another benefit is having a master microservice that owns a certain type of data per entity so that updates and queries for that type of data are driven only by that microservice

+ pag. 46/297 -> **Direct client-to-microservice communication versus the API Gateway pattern**
  - APPROACH ONE -> **Direct client-to-microservice communication**
    - each microservice has a public endpoint, sometimes with a different TCP port for each microservice.

  - APPROACH TWO -> **Using an API Gateway**
    - This is a service that provides a single entry point for certain groups of microservices
    - the API Gateway would be implemented as a custom Web API service running as a container.

+ pag. 50/297 -> **Communication between microservices**
   - we use interprocess communication protocols

+ pag. 51/297 -> **Communication types**
   - The first axis is defining if the protocol is synchronous or asynchronous
   - The second axis is defining if the communication has a single receiver or multiple receivers.
      - Multiple receivers. Each request can be processed by zero to multiple receivers. This type of communication must be asynchronous.

+ pag. 53/297 -> **Communication styles**
   - synchronous request/response-based communication mechanism, protocols such as HTTP and REST approaches are the most common
     - When a client uses request/response communication, it assumes that the response will arrive in a short time, typically less than a second, or a few seconds at most. For delayed responses, you need to implement asynchronous communication based on messaging patterns and messaging technologies
   - If you are communicating between services internally (within your Docker host or microservices cluster) you might also want to use binary format communication mechanisms (like Service Fabric remoting or WCF using TCP and binary format)
   - Alternatively, you can use asynchronous, message-based communication mechanisms such as AMQP.

+ pag. 54/297 -> **Push and real-time communication based on HTTP**
   
+ pag. 55/297 -> **Asynchronous message-based communication**
   - That means that when changes occur, you need some way to reconcile changes across the different models. A solution is eventual consistency and event-driven communication based on asynchronous messaging.

**IMPORTANT RULE IN COMMUNICATION**: use only asynchronous messaging between the internal services, and to use synchronous communication (such as HTTP) only from the client apps to the front-end services (API Gateways plus the first level of microservices).

+ pag. 55/297 -> **ASYNCHRONOUS Single-receiver message-based communication**

=========================================================
**SECTION 5
Development Process for Docker-Based Applications**


+ pag. 76/297 -> **Workflow for developing Docker container-based applications**

+ pag. 78/297 
   - You need a Dockerfile for each custom image you want to build; you also need a Dockerfile for each container to be deployed.
   - If your application contains a single custom service, you need a single Dockerfile. If your application contains multiple services (as in a microservices architecture), you need one Dockerfile for each service.
  - The **Dockerfile** is placed in the root folder of your application or service. It contains the commands that tell Docker how to set up and run your application or service in a container.

+ pag. 79/297 -> Example of a docker file

+ pag. 81/297 -> Create Images Defined at Dockerfile
   - need to create the Docker images and deploy containers to a local Docker host (Windows or Linux VM) and run, test, and debug against those local containers.
   - The **docker-compose.yml** file lets you define a set of related services to be deployed as a composed application

+ pag.85/297 -> **Build and run your Docker application**