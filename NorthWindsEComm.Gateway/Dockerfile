FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NorthWindsEComm.Gateway/NorthWindsEComm.Gateway.csproj", "NorthWindsEComm.Gateway/"]
RUN dotnet restore "NorthWindsEComm.Gateway/NorthWindsEComm.Gateway.csproj"
COPY . .
WORKDIR "/src/NorthWindsEComm.Gateway"
RUN dotnet build "NorthWindsEComm.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NorthWindsEComm.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthWindsEComm.Gateway.dll"]
