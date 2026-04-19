FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /

COPY ["src/WebDiffy.UI/WebDiffy.UI.csproj", "src/WebDiffy.UI/"]
COPY ["src/WebPageChangeMonitor.Models/WebPageChangeMonitor.Models.csproj", "src/WebPageChangeMonitor.Models/"]
COPY ["src/WebPageChangeMonitor.Common/WebPageChangeMonitor.Common.csproj", "src/WebPageChangeMonitor.Common/"]

RUN dotnet restore "src/WebDiffy.UI/WebDiffy.UI.csproj"

COPY . .
WORKDIR "src/WebDiffy.UI/"

RUN dotnet publish "WebDiffy.UI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

EXPOSE 5011
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WebDiffy.UI.dll"]