# Harri.SchoolDemoAPI - ASP.NET Core 8.0 Server

Demo .NET 8 API about students, schools, and students applications to schools

This repository is intended as a demonstration of a RESTful API with a SQL Server database focusing on automated testing to validate the API functionality.

## WIP - API
So far the /students/ api is complete: [StudentsApiController.cs](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Harri.SchoolDemoAPI/Controllers/StudentsApiController.cs)

Using:
  - [Dapper](https://github.com/DapperLib/Dapper)
  - [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
  - [RestSharp](https://github.com/restsharp/RestSharp) for the client
  - 
# Running the SchoolDemo REST Web API
You have three options for running this web api,

1. Build it from source and run with dotnet sdk. See [Building from source](#building-from-source) below
2. Build a docker container and run. See [Building container from source](#building-container-from-source) below
3. Pull and run a docker container. See [Running from container](#running-from-container) below

   For all options above you will also need to pull and run the [database container](#running-the-database-from-container)

## Building from source
Use the included build scripts in the root of the repo or build in Visual Studio
### Windows

#### PowerShell
>`./build.bat`
#### CMD
>`build`

### Linux
>`./build.sh`

## Running from source
The build script then prompts you to run using
> `dotnet run --project src\Harri.SchoolDemoAPI\Harri.SchoolDemoAPI.csproj`

The API will be accessible via http://localhost:8080 by default

Also make sure to set up the database or the api will return 500 Internal server error
[Running the database from container](#running-the-database-from-container)

## Building container from source
Build the container locally run: 

> `docker build -t schooldemoapi -f .\src\Harri.SchoolDemoAPI\Dockerfile .`

## Running from container
If you built the container yourself locally as above run:

> `docker run -it -p 8080:8080 --name schooldemoapi schooldemoapi`

---

If you don't want to build the container you can pull the latest main branch linux container image from the [DockerHub harri-schooldemoapi repository](https://hub.docker.com/repository/docker/harrisonslater/harri-schooldemoapi/general)

> `docker pull harrisonslater/harri-schooldemoapi:latest`

And run 

> `docker run -it -p 8080:8080 --name schooldemoapi harrisonslater/harri-schooldemoapi:latest`

The API will be accessible via http://localhost:8080 or you can specify a different port in the docker run command above like `5000:8080`

## Running the database from container
The database required by the SchoolDemoAPI is available as a linux container image prefilled with student, school, and application data from the [DockerHub harri-schooldemosql-database repository](https://hub.docker.com/repository/docker/harrisonslater/harri-schooldemosql-database/general)

> `docker pull harrisonslater/harri-schooldemosql-database:latest`

And to run the database container:

> `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=p@ssw0rd" -p 1433:1433 -d harrisonslater/harri-schooldemosql-database:latest`

## See Contract Tests 
[Contract Test README.md](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Contract/README.md)

Using:
  - [Pact Net](https://github.com/pact-foundation/pact-net)
  - [NUnit](https://github.com/nunit/nunit)
  - [Moq](https://github.com/devlooped/moq)
  - [FluentAssertions](https://github.com/fluentassertions/fluentassertions)

## See Integration Tests
[StudentApiTests.cs](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Harri.SchoolDemoAPI.Tests.Integration/StudentApiTests.cs)

Integration tests are run in-agent using a preconfigured containerised SQL server: [harri-schooldemosql-database](https://hub.docker.com/repository/docker/harrisonslater/harri-schooldemosql-database/general)

## Build pipeline
Azure DevOps pipeline defined [in yaml](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/pipeline/azure-pipelines.yml)
