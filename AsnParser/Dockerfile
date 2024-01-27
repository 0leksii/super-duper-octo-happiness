FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY AsnParser AsnParser
COPY AsnParser/appsettings.Docker.json AsnParser/appsettings.json
COPY AsnParserTests AsnParserTests
COPY AsnParser.sln AsnParser.sln

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
RUN mkdir -p /tmp/spooler
ENTRYPOINT ["dotnet", "AsnParser.dll"]