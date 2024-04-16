# Harri.SchoolDemoAPI - ASP.NET Core 8.0 Server

Demo API about students, schools, and students applications to schools

## See Contract Tests 
https://github.com/HarrisonSlater/Harri.SchoolDemoApi/blob/main/src/Tests/Contract/README.md

## Upgrade NuGet Packages

NuGet packages get frequently updated.

To upgrade this solution to the latest version of all NuGet packages, use the dotnet-outdated tool.


Install dotnet-outdated tool:

```
dotnet tool install --global dotnet-outdated-tool
```

Upgrade only to new minor versions of packages

```
dotnet outdated --upgrade --version-lock Major
```

Upgrade to all new versions of packages (more likely to include breaking API changes)

```
dotnet outdated --upgrade
```


## Run

Linux/OS X:

```
sh build.sh
```

Windows:

```
build.bat
```
## Run in Docker

```
cd src/Harri.SchoolDemoAPI
docker build -t harri.schooldemoapi .
docker run -p 5000:8080 harri.schooldemoapi
```
