#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Copy the code into the container
WORKDIR /src
COPY ["src/Harri.SchoolDemoAPI.Models/Harri.SchoolDemoAPI.Models.csproj", "Harri.SchoolDemoAPI.Models/"]
COPY ["src/Harri.SchoolDemoAPI/Harri.SchoolDemoAPI.csproj", "Harri.SchoolDemoAPI/"]

# NuGet restore
RUN dotnet restore "Harri.SchoolDemoAPI.Models/Harri.SchoolDemoAPI.Models.csproj"
COPY ["src/Harri.SchoolDemoAPI.Models/", "Harri.SchoolDemoAPI.Models/"]

RUN dotnet restore "Harri.SchoolDemoAPI/Harri.SchoolDemoAPI.csproj"
COPY ["src/Harri.SchoolDemoAPI/", "Harri.SchoolDemoAPI/"]

# Build the API
WORKDIR "Harri.SchoolDemoAPI"
RUN dotnet build "Harri.SchoolDemoAPI.csproj" -c Release -o /app/build



# Publish it
FROM build AS publish
RUN dotnet publish "Harri.SchoolDemoAPI.csproj" -c Release -o /app/publish



# Make the final image for publishing
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Harri.SchoolDemoAPI.dll"]


