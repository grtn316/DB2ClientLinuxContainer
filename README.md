# DB2ClientLinuxContainer

This project is an example of how to use the .net core IBM DB2 nuget package inside a linux container.

- To start, you will need to deploy a sample DB2 database:

    - ```docker
        docker pull ibmcom/db2
        docker run -itd --name mydb2 --privileged=true -p 50000:50000 -e LICENSE=accept -e DB2INST1_PASSWORD=<choose an instance password> --SAMPLEDB=true ibmcom/db2
        ```

- After the datbase is running (takes a bit to start), then you can launch the Visual Studio project and begin testing.

- Once the project is open, right click on the project and select "**Manage User Secrets**" and enter the following:
    - ```json
        {
            "username": "db2inst1",
            "password": "<your instance password>"
        }
        ```


- The docker file is defaulted to **Debug Mode** and you will need to comment/uncomment the alternate lines for a release deployment.
    - ```docker
            FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
            WORKDIR /app

            ENV LD_LIBRARY_PATH="/app/bin/Debug/netcoreapp3.1/clidriver/lib"   <-----DEBUG
            #ENV LD_LIBRARY_PATH="/app/clidriver/lib"   <-----RELEASE
            RUN set -e; \
            apt-get update; \
            apt-get install -y libxml2-dev;

            FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
            WORKDIR /src
            COPY ["DB2Client/DB2Client.csproj", "DB2Client/"]
            RUN dotnet restore "DB2Client/DB2Client.csproj"
            COPY . .
            WORKDIR "/src/DB2Client"
            RUN dotnet build "DB2Client.csproj" -c Debug -o /app/build   <-----DEBUG
            #RUN dotnet build "DB2Client.csproj" -c Release -o /app/build   <-----RELEASE

            FROM build AS publish
            RUN dotnet publish "DB2Client.csproj" -c Debug -o /app/publish   <-----DEBUG
            #RUN dotnet publish "DB2Client.csproj" -c Release -o /app/publish   <-----RELEASE

            FROM base AS final
            WORKDIR /app
            COPY --from=publish /app/publish .
            Env PATH=$PATH:"/app/bin/Debug/netcoreapp3.1/clidriver/bin:/app/bin/Debug/netcoreapp3.1/clidriver/lib"   <-----DEBUG
            #Env PATH=$PATH:"/app/clidriver/bin:/app/clidriver/lib"   <-----RELEASE
            ENTRYPOINT ["dotnet", "DB2Client.dll"]
        ```
- Make sure the Docker Debug profile is selected and run the project.
  - After the project is done running, you should see output like below in the output console:
    - ```
        1000
        1001
        1002
        1003
        1004
        1005
        ```