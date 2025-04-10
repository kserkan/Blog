# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# 1. Build aşaması (SDK imajı)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"


# 2. Proje dosyasını kopyala ve restore işlemi yap
COPY Blog.csproj ./
RUN dotnet restore Blog.csproj

# 3. Tüm dosyaları kopyala ve build işlemi
COPY . ./
RUN dotnet publish Blog.csproj -c Release -o /app/publish /p:UseAppHost=false

# 4. Runtime aşaması (ASP.NET Core runtime imajı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 5. Yayınlanan dosyaları kopyala
COPY --from=build /app/publish .

# 6. Gerekli portu aç
EXPOSE 80

# 7. Uygulamayı başlat
ENTRYPOINT ["dotnet", "Blog.dll"]
