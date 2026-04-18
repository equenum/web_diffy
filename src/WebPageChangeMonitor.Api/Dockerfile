FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "WebPageChangeMonitor.Api"]