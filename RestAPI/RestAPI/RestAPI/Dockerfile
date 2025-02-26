# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore --disable-parallel

COPY . .
RUN dotnet publish -c Release -o /out

# 2. Serve stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080 8081

ENV ASPNETCORE_URLS="http://+:8080"
ENV ASPNETCORE_HTTP_PORT="https://+:8081"
ENV ASPNETCORE_ENVIRONMENT="Production"

ENTRYPOINT ["dotnet","RestAPI.dll"]
