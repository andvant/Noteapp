FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY src/Api Api/
COPY src/Core Core/
COPY src/Infrastructure Infrastructure/

WORKDIR Api
RUN dotnet restore
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/Api/publish .
EXPOSE 80
# ENV ASPNETCORE_ENVIRONMENT Development # set in docker-compose
# ENV ASPNETCORE_URLS http://+:80 # set in docker-compose
ENTRYPOINT [ "dotnet", "Api.dll" ]