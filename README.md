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


### BOOK SECTION 4
"**Architecting Container- and Microservice-Based Applications**"


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


### BOOK SECTION 5
"**Development Process for Docker-Based Applications**"

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


### BOOK SECTION 6
"**Deploying Single-Container-Based .NET Core Web Applications on Linux or Windows Nano Server Hosts**"

- pag. 91/297
  - You can use Docker containers for monolithic deployment of simpler web applications.
 
- pag. 92/297 "**eShopWeb**" its an example of MVC app with separated container for SQL database.


### BOOK SECTION 7
"**Migrating Legacy Monolithic .NET Framework Applications to Windows Containers**"

+ pag. 96/297 the book does an example of an old legacy webapp and how to contenarize it


### BOOK SECTION 8
"**Designing and Developing Multi-Container and Microservice-Based .NET Applications**"

+ pag. 103/297 **application apecifications**

+ pag. 104/297 **choosing an architecture**

+ pag. 106/297 **Communication Architecture**
   - Direct client-to-microservice communication. This is used for queries and when accepting update or transactional commands from the client apps.
   - Asynchronous event-based communication. This occurs through an event bus to propagate updates across microservices or to integrate with external applications. The event bus can be implemented with any messaging-broker infrastructure technology like RabbitMQ, or using higher-level service buses like Azure Service Bus, NServiceBus, MassTransit, or Brighter.

However, if you are going to design a large microservice-based application with dozens of microservices, we strongly recommend that you consider the API Gateway pattern, as we explained in the architecture section.

+ pag 107/297 **Benefits of a microservice-based solution**
   - Each microservice is relatively small—easy to manage and evolve.
   - It is possible to scale out individual areas of the application
   - ..

+ pag 107/297 **Downsides of a microservice-based solution**
   - Issues with direct client‑to‑microservice communication. When the application is large, with dozens of microservices, there are challenges and limitations if the application requires direct client-to-microservice communications. One problem is a potential mismatch between the needs of the client and the APIs exposed by each of the microservices. In certain cases, the client application might need to make many separate requests to compose the UI, which can be inefficient over the Internet and would be impractical over a mobile network. Therefore, requests from the client application to the back-end system should be minimized.
   - difficult to do refactoring.
   - partitioning the microservices.

**VERY IMPORTANT**
+ pag. 110/297 **The new world: multiple architectural patterns and polyglot microservices**:
**
   - Simple CRUD, single-tier, single-layer.
   - Traditional N-Layered.
   - Domain-Driven Design N-layered.
   - Clean Architecture (as used with eShopOnWeb)
   - Command and Query Responsibility Segregation (CQRS).
   - Event-Driven Architecture (EDA).
**

+ pag. 111/297 **Creating a simple data-driven CRUD microservice**

+ pag. 119/297 **Generating Swagger description metadata from your ASP.NET Core Web API**
   - Swagger is a commonly used open source framework backed by a large ecosystem of tools that helps you design, build, document, and consume your RESTful APIs.
   - Swagger’s metadata is what Microsoft Flow, PowerApps, and Azure Logic Apps use to understand how to use APIs and connect to them.
 
+ pag. 123/297 **Defining your multi-container application with docker-compose.yml**
   - Basically, you define each of the containers you want to deploy plus certain characteristics for each container deployment. Once you have a multi-container deployment description file, you can deploy the whole solution in a single action orchestrated by the docker-compose up CLI command, or you can deploy it transparently from Visual Studio.
  - **build:** key setting in the file. This means that the image must have been previously built (with docker build) or have been downloaded (with the docker pull command) from any Docker registry.
  
+ pag.128/297 **Targeting multiple environments**
   - A typical use case is when you define multiple compose files so you can target multiple environments, like production, staging, CI, or development. To support these differences, you can split your Compose configuration into multiple files, as shown in Figure 8-12.

+ pag 138/297 **Using a database server running as a container** 
   - The important point in microservices is that each microservice owns its related data, therefore its related SQL database in this case. But the databases can be anywhere.

+ pag. 142/297 **redis**
   - show how to declare the **docker-compose.yml**. It defines a container named basket.data based on the redis image and publishing the port 6379 internally, meaning that it will be accessible only from other containers running within the Docker host.
     - then in the docker-compose.override.yml file, the basket.api microservice for the eShopOnContainers sample defines the connection string to use for that Redis container
```
      basket.api:
          environment:
          // Other data ...
          - ConnectionString=basket.data
          - EventBusConnection=rabbitmq
```

+ pag. 142/297 **Implementing event-based communication between microservices (integration events)** 
   - **Event Bus**: As described earlier, when you use event-based communication, a microservice publishes an event when something notable happens, such as when it updates a business entity. Other microservices subscribe to those events. When a microservice receives an event, it can update its own business entities, which might lead to more events being published. This publish/subscribe system is usually performed by using an implementation of an event bus.

+ pag. 144/297 **Integration events** + **Event Bus**
   - each microservice has an integration event class.
     - it's possible publish the integration event outside the microservice, then an handler in each subscriber microservice manage the event. 
       - the **Event Bus** allows publish/subscribe-style communication between microservices without requiring the components to explicitly be aware of each other

The event bus is related to the Observer pattern and the publish-subscribe pattern.
  - **Observer pattern**: In the Observer pattern, your primary object (known as the Observable) notifies other interested objects (known as Observers) with relevant information (events).
  - **Publish/Subscribe pattern**: The purpose of the Pub/Sub pattern is the same as the Observer pattern: you want to notify other services when certain events take place. But there is an important semantic difference between the Observer and Pub/Sub patterns. In the Pub/Sub pattern, the focus is on broadcasting messages. In contrast, in the Observer pattern, the Observable does not know who the events are going to, just that they have gone out. In other words, the Observable (the publisher) does not know who the Observers (the subscribers) are.
  - **The Middleman or Event Bus**: How do you achieve anonymity between publisher and subscriber? An easy way is let a middleman take care of all the communication. An event bus is one such middleman.
An event bus is typically composed of two parts:
• The abstraction or interface.
• One or more implementations.

+ pag. 146/297 **How the Event Bus works and it's Interface**
```
public interface IEventBus
{
void Publish(IntegrationEvent @event);
void Subscribe<T>(IIntegrationEventHandler<T> handler)
where T: IntegrationEvent;
void Unsubscribe<T>(IIntegrationEventHandler<T> handler)
where T : IntegrationEvent;
}
```
  - **Publish** method(used by the microservice that is publishing the event): The event bus will broadcast the integration event passed to it to any microservice subscribed to that event. 
  - **Subscribe** method: The Subscribe method is used by the microservices that want to receive events. This method has two parts. The first is the integration event to subscribe to (IntegrationEvent). The second part is the integration event handler (or callback method) to be called (IIntegrationEventHandler<T>) when the microservice receives that integration event message.

+ pag.147/297 **Implementing an event bus with RabbitMQ for the development or test environment**
  - Some code examples are done in the book.

+ pag. 152/297 **Designing atomicity and resiliency when publishing to the event bus**
  - the book talk about **inconsistency**: Let’s go back to the initial issue and its example. If the service crashes after the database is updated (in this case, right after the line of code with _context.SaveChangesAsync()), but before the integration event is published, the overall system could become inconsistent. This might be business critical, depending on the specific business operation you are dealing with.
  - an option can be: Notice that with this approach, you are persisting only the integration events for each origin microservice, and only the events that you want to communicate to other microservices or external systems.

+ pag.159/297 **Idempotency**


### BOOK SECTION 9 - DDD, CQRS, DTO, POCO
"**Tackling Business Complexity in a Microservice with DDD and CQRS Patterns**"

+ pag. 166/297 -> **domain-driven design (DDD) and Command and Query Responsibility Segregation (CQRS) approaches**
  - CQRS: is an architectural pattern that separates the models for reading and writing data. The basic idea is that you can divide a system’s operations into two sharply separated categories:
• Queries. These return a result and do not change the state of the system, and they are free of side effects.
• Commands. These change the state of a system.
 The separation aspect of CQRS is achieved by grouping query operations in one layer and commands in another layer.

CQRS and DDD patterns describe something inside a single system or component; in this case, something inside a microservice.

+ pag.170/297 
   - AGGREGATE PATTERN: Aggregate pattern, you treat many domain objects as a single unit as a result of their relationship in the domain.

+ pag. 171/297 -> **Implementing reads/queries in a CQRS microservice**
   - DTO: The returned data (ViewModel) can be the result of joining data from multiple entities or tables in the database, or even across multiple aggregates defined in the domain model for the transactional area. In this case, because you are creating queries independent of the domain model, the aggregates boundaries and constraints are completely ignored and you are free to query any table and column you might need.

+ pag. 174/297 -> **Designing a DDD-oriented microservice**
   - DDD talks about problems as domains. It describes independent problem areas as Bounded Contexts (each Bounded Context correlates to a microservice)
   - **Keep the microservice context boundaries relatively small**: in this section there is a good description of how organize boundaries.
   - **Layers in DDD microservices**
        - POCO: Following the Persistence Ignorance and the Infrastructure Ignorance principles, this layer must completely ignore data persistence details. These persistence tasks should be performed by the infrastructure layer. Therefore, this layer should not take direct dependencies on the infrastructure, which means that an important rule is that your domain model entity classes should be POCOs.
Domain entities should not have any direct dependency (like deriving from a base class) on any data access infrastructure framework like Entity Framework or NHibernate. Ideally, your domain entities should not derive from or implement any type defined in any infrastructure framework.

+ **APPLICATION LAYERS**:
   - **Domain Model Layer**: Responsible for representing concepts of the business, information about the business situation, and business rules. State that reflects the business situation is controlled and used here, even though the technical details of storing it are delegated to the infrastructure. This layer is the heart of business software.
   - **Application Layer**. go in the book to see my notes
   - **Infrastructure Layer** go in the book to see my notes

+ pag. 178/297 **Designing a microservice domain model**
   - **The Domain Entity pattern**: An entity’s identity can cross multiple microservices or Bounded Contexts. there is an example of "Domain Entity Data"
      - entity's data
      - entity's behaviour and logic
      MORE details in the pdf.
   - **Rich domain model versus anemic domain model**(pag.179):
      The **anemic domain model** is just a procedural style design. Anemic entity objects are not real objects because they lack behavior (methods). They only hold data properties and thus it is not object-oriented design. By putting all the behavior out into service objects (the business layer) you essentially end up with spaghetti code or transaction scripts, and therefore you lose the advantages that a domain model provides.
        - usually this model works for CRUD domains     
   - the **Aggregate Pattern**(pag. 181): A more fine-grained DDD unit is the aggregate, which describes a cluster or group of entities and behaviors that can be treated as a cohesive unit.
        - more details in the pdf.

+ pag. 183/297 -> **Implementing a microservice domain model with .NET Core**
   - he shows how to organize folder structured by aggregates
   - **Implementing domain entities as POCO classes**(pag. 185): POCO class. It does not have any direct dependency on Entity Framework Core or any other infrastructure framework. This implementation is as it should be, just C# code implementing a domain model.
   - **Having an aggregate root means that most of the code related to consistency and business rules of the aggregate’s entities should be implemented as methods in the Order aggregate root class.**
   - **IMPORTANT**: to follow **DDD patterns**, entities must not have public setters in any entity property. Changes in an entity should be driven by explicit methods with explicit ubiquitous language about the change they are performing in the entity.
       - for the same reason, collections within the entity should be read-only properties.
           - pag.186 and 187 there are two example of wrong and then **right approach to separate domain layer from business layer in DDD approach**

+ pag. 190/297 -> **Seedwork (reusable base classes and interfaces for your domain model)**
   - This folder contains custom base classes that you can use as a base for your domain entities and value objects, so you do not have redundant code in each domain’s object class.

+ pag. 192/297 --> **Implementing Value Objects**
   - As discussed in earlier sections about entities and aggregates, identity is fundamental for entities. However, there are many objects and data items in a system that do not require an identity and identity tracking, such as value objects.
   - Value object describe a complex value
   - **They have no identity**.
   - **They are immutable**: The values of a value object must be immutable once the object is created. Therefore, when the object is constructed, you must provide the required values, but you must not allow them to change during the object’s lifetime.
  
The immutable nature allow them to be reused.

+ pag. 197/297 --> **Using Enumeration classes instead of C# language enum types**

+ pag. 200/297 --> **Implementing validations in the domain model layer**

+ pag. 200/297 --> **Using validation attributes in the model based on data annotations**

+ pag. 201/297 --> **Validating entities by implementing the Specification pattern and the Notification pattern**
 
+ pag. 204/297 --> **Domain events: design and implementation** 
   - what is a domain event?

+ pag. 204/297 --> **Domain events versus integration events**
   - Domain events as a preferred way to trigger side effects across multiple aggregates within the same domain(pag. 205)
      - It is important to understand that this event-based communication is not implemented directly within the aggregates; you need to implement domain event handlers.
      - The event handlers are typically in the application layer

+ pag. 208/297 --> **Implementing domain events**, **Raising domain events**
   - **The deferred approach for raising and dispatching events**(pag. 209)
   - **The domain event dispatcher: mapping from events to event handlers**(pag.213)

+ **IMPORTANTE** 
at page 213 there is a schema with representing the all pattern of the domain event dispatcher

+ pag. 217/297 --> **Designing the infrastructure persistence layer**
   - **Repository Pattern**
      - We must emphasize again that only one repository should be defined for each aggregate root, as shown in Figure 9-17. To achieve the goal of the aggregate root to maintain transactional consistency between all the objects within the aggregate, you should never create a repository for each table in the database.
      - **USEFUL diagram on page 218**
      - It's useful for UNIT TESTS: unit test against domain model and its domain logic.

+ pag 219/297 --> **The difference between the Repository pattern and the legacy Data Access class (DAL class) pattern**
   - **Data Access Object**: perform directly persistence operations against storage
   - **Unit Of Work**: for a specific user action, perform in a singular transaction many crud operations. In my case is implemented in the **DbContext**.

+ pag. 221/297 --> **Implementing the infrastructure persistence layer with Entity Framework Core**
   - Fluent API and the OnModelCreating method(pag. 229)
   - A benefit when using NoSQL databases is that the entities are more denormalized, so you do not set a table mapping. Your domain model can be more flexible than when using a relational database.

+ pag. 236/297 --> **Designing the microservice application layer and Web API**
   - Using SOLID principles and Dependency Injection

+ pag. 241/297 --> **Implementing the Command and Command Handler patterns**
   - details in the pdf
   - **The Command process pipeline: how to trigger a command handler**: so here is described the logic to interact with a command handler from a controller. One option is:
      - Using the Mediator pattern (in-memory) in the command pipeline.

In the initial version of eShopOnContainers, we decided to use synchronous command processing, started from HTTP requests and driven by the Mediator pattern. That easily allows you to return the success or failure of the process, as in the CreateOrderCommandHandler implementation.

+ pag. 250/297 --> **Implementing the command process pipeline with a mediator pattern (MediatR)**
   - As a sample implementation, this guide proposes using the in-process pipeline based on the Mediator pattern to drive command ingestion and routing them, in memory, to the right command handlers. The guide also proposes applying decorators or behaviors in order to separate cross-cutting concerns.


### BOOK SECTION 10
"**Implementing Resilient Applications**"

Scope: Your microservice and cloud based applications must embrace the partial failures that will certainly occur eventually. You need to design your application so it will be resilient to those partial failures.
Resiliency is the ability to recover from failures and continue to function.
The goal of resiliency is to return the application to a fully functioning state after a failure.

+ pag. 258/297 --> **Strategies for handling partial failure**
   - Use asynchronous communication (for example, message-based communication) across internal microservices. It is highly advisable not to create long chains of synchronous HTTP calls across the internal microservices because that incorrect design will eventually become the main cause of bad outages. On the contrary, except for the front-end communications between the client applications and the first level of microservices or fine-grained API Gateways, it is recommended to use only asynchronous (message-based) communication once past the initial request/response cycle, across the internal microservices. Eventual consistency and event-driven architectures will help to minimize ripple effects. These approaches enforce a higher level of microservice autonomy

+ pag. 260/297 **Implementing resilient Entity Framework Core SQL connections**
   - it's code to specify in "Startup.cs" that at EF Core connection level enables resilient SQL connections that are retried if the connection fails