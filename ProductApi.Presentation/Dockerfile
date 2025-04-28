# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ECommerces.Product.ApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj", "ECommerces.Product.ApiSolution/ProductApi.Presentation/"]
COPY ["DemoECommerce.SharedLibrarySolution/ECommmerce.SharedLibrary/ECommmerce.SharedLibrary.csproj", "DemoECommerce.SharedLibrarySolution/ECommmerce.SharedLibrary/"]
COPY ["ECommerces.Product.ApiSolution/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj", "ECommerces.Product.ApiSolution/ProductApi.Infrastructure/"]
COPY ["ECommerces.Product.ApiSolution/ProductApi.Application/ProductApi.Application.csproj", "ECommerces.Product.ApiSolution/ProductApi.Application/"]
COPY ["ECommerces.Product.ApiSolution/ProductApi.Domain/ProductApi.Domain.csproj", "ECommerces.Product.ApiSolution/ProductApi.Domain/"]
RUN dotnet restore "./ECommerces.Product.ApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj"
COPY . .
WORKDIR "/src/ECommerces.Product.ApiSolution/ProductApi.Presentation"
RUN dotnet build "./ProductApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductApi.Presentation.dll"]