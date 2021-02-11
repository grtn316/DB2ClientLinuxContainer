#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

ENV LD_LIBRARY_PATH="/app/bin/Debug/netcoreapp3.1/clidriver/lib"
#ENV LD_LIBRARY_PATH="/app/clidriver/lib"
RUN set -e; \
apt-get update; \
apt-get install -y libxml2-dev;

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DB2Client/DB2Client.csproj", "DB2Client/"]
RUN dotnet restore "DB2Client/DB2Client.csproj"
COPY . .
WORKDIR "/src/DB2Client"
RUN dotnet build "DB2Client.csproj" -c Debug -o /app/build
#RUN dotnet build "DB2Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DB2Client.csproj" -c Debug -o /app/publish
#RUN dotnet publish "DB2Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
Env PATH=$PATH:"/app/bin/Debug/netcoreapp3.1/clidriver/bin:/app/bin/Debug/netcoreapp3.1/clidriver/lib"
#Env PATH=$PATH:"/app/clidriver/bin:/app/clidriver/lib"
ENTRYPOINT ["dotnet", "DB2Client.dll"]