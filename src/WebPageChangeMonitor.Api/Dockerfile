FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /

COPY ["src/WebPageChangeMonitor.Api/WebPageChangeMonitor.Api.csproj", "src/WebPageChangeMonitor.Api/"]
COPY ["src/WebPageChangeMonitor.Models/WebPageChangeMonitor.Models.csproj", "src/WebPageChangeMonitor.Models/"]
COPY ["src/WebPageChangeMonitor.Common/WebPageChangeMonitor.Common.csproj", "src/WebPageChangeMonitor.Common/"]
COPY ["src/WebPageChangeMonitor.Services/WebPageChangeMonitor.Services.csproj", "src/WebPageChangeMonitor.Services/"]
COPY ["src/WebPageChangeMonitor.Data/WebPageChangeMonitor.Data.csproj", "src/WebPageChangeMonitor.Data/"]

RUN dotnet restore "src/WebPageChangeMonitor.Api/WebPageChangeMonitor.Api.csproj"

COPY . .
WORKDIR "src/WebPageChangeMonitor.Api/"

RUN dotnet publish "WebPageChangeMonitor.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

EXPOSE 5180
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WebPageChangeMonitor.Api.dll"]