# ============================================
# 1. BUILD STAGE
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY . .

# Restaurar paquetes
RUN dotnet restore "BDJoinSN.API/BDJoinSN.API.csproj"

# Publicar la aplicación
RUN dotnet publish "BDJoinSN.API/BDJoinSN.API.csproj" -c Release -o /app/publish

# ============================================
# 2. RUNTIME STAGE
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Configurar puerto para Render
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Exponer puerto
EXPOSE 8080

# Punto de entrada
ENTRYPOINT ["dotnet", "BDJoinSN.API.dll"]