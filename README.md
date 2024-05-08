# Harri.SchoolDemoAPI - ASP.NET Core 8.0 Server

Demo .NET 8 API about students, schools, and students applications to schools

This repository is intended as a demonstration of a RESTful API with a SQL Server database focusing on automated testing to validate the API functionality.

## WIP - API
So far the /students/ api is complete: https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Harri.SchoolDemoAPI/Controllers/StudentsApiController.cs
Using:
  - [Dapper](https://github.com/DapperLib/Dapper)
  - [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
  - [RestSharp](https://github.com/restsharp/RestSharp) for the client

## See Contract Tests 
https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Contract/README.md

Using:
  - [Pact Net](https://github.com/pact-foundation/pact-net)
  - [NUnit](https://github.com/nunit/nunit)
  - [Moq](https://github.com/devlooped/moq)
  - [FluentAssertions](https://github.com/fluentassertions/fluentassertions)

## See Integration Tests
https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Harri.SchoolDemoAPI.Tests.Integration/StudentApiTests.cs

Integration tests are run in-agent using a containerised SQL server: https://hub.docker.com/repository/docker/harrisonslater/schooldemo_sqldatabase/general

## Build pipeline
Azure DevOps pipeline defined [in yaml](https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/azure-pipelines.yml)
