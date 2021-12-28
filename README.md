# reservations-microservice

This microservice is part of the Charging Stations Application and provides endpoints for retrieving, creating, updating and deleting Reservations.

# Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/vs/)
- [.NET 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)

# Setup

Open the .sln file with Visual Studio and restore NuGet packages:

    Right click Solution -> Restore NuGet Packages

Open Package Manager Console and execute these commands to create the database and it's tables:

    $env:DB_HOST = "{Host}";
    $env:DB_NAME = "{Database}";
    $env:DB_USERNAME = "{Username}";
    $env:DB_PASSWORD= "{Password}";
    Update-Database;

When running locally with Docker, move Dockerfile to the Reservations.API project and add Environment Variables in one line as follows:

    ENV DB_HOST={Host} DB_NAME={Database} DB_USERNAME={Username} DB_PASSWORD={Password}


# Details

## Frameworks & Tools

This service is developed in the .NET 5.0 Framework, using Visual Studio 2019 as the IDE of choice. Entity Framework Core is used as an ORM and to generate the database from defined entity classes.

## Continuous Integration

A GitHub Actions CI pipeline is set up for this service, which builds a Docker Image from the project and pushes it to DockerHub: https://hub.docker.com/repository/docker/anzx10/reservations-microservice.

## Database

This service is using a PostgreSQL Database, hosted on ElephantSQL. It consists of 2 tables for the following entities:
- **Reservation**: A reservation of a specific charger made by a consumer. Past reservations are preserved in the database.
- **Status**: A predefined set of possible Reservation Statuses.

## Configuration

This microservice is configured using Environment Variables. Currently, only configuration for the database is required. The password is safely stored in Kubernetes Secrets, while other information is provided during the deployment process on the Cluster.

## Endpoints

- GET /Reservations
- GET /Reservations/{id}
- POST /Reservations
- PUT /Reservations/{id}
- DELETE /Reservations/{id}

All endpoints are described with the OpenAPI 3 standard on the **/swagger/index.html** endpoint of the service. The API is also versioned, which adds a version segment to the path of the endpoints (e.g. api/**v1**/Reservations).