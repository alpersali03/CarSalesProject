FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CarSalesSystem ./CarSalesSystem

WORKDIR /src/CarSalesSystem
RUN dotnet restore CarSalesSystem.csproj
RUN dotnet publish CarSalesSystem.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/App_Data

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "CarSalesSystem.dll"]
