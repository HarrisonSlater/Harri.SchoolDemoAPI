FROM mcr.microsoft.com/mssql/server:2022-latest AS base
EXPOSE 1433
ENV ACCEPT_EULA=Y

#For a production database the -P p@ssW0rd should be injected via docker secrets
HEALTHCHECK CMD /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P p@ssw0rd -Q 'SELECT 1' -C -N || exit 1
