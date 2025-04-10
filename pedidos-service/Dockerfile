#Este es el dockerfile que nos deja vs por defecto. 

# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

## Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["pedidos-service/pedidos-service.csproj", "pedidos-service/"]
RUN dotnet restore "./pedidos-service/pedidos-service.csproj"
COPY . .
WORKDIR "/src/pedidos-service"
RUN dotnet build "./pedidos-service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./pedidos-service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pedidos-service.dll"]


## modifico por este dockerfile mas simple. Pero el otro es mas rapido para ambiente dev.

# Imagen base para compilar la API
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app
#
## Copiar archivos del proyecto y restaurar dependencias
#COPY *.csproj ./
#RUN dotnet restore
#
## Copiar todo el código y compilar la aplicación
#COPY . ./
#RUN dotnet publish -c Release -o out
#
## Imagen base para ejecutar la API
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app/out .
#
## Exponer el puerto 8080 en el contenedor
#EXPOSE 8080
#ENV ASPNETCORE_URLS=http://+:8080
#
## Comando para ejecutar la API
#ENTRYPOINT ["dotnet", "pedidos-service.dll"]