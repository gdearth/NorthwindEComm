FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NorthWindsEComm.Suppliers.Api/NorthWindsEComm.Suppliers.Api.csproj", "NorthWindsEComm.Suppliers.Api/"]
RUN dotnet restore "NorthWindsEComm.Suppliers.Api/NorthWindsEComm.Suppliers.Api.csproj"
COPY . .
WORKDIR "/src/NorthWindsEComm.Suppliers.Api"
RUN dotnet build "NorthWindsEComm.Suppliers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NorthWindsEComm.Suppliers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthWindsEComm.Suppliers.Api.dll"]
