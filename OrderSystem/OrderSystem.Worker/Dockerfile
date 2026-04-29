# build stage
# Two steps of build COPY and RUN are used to leverage Docker's layer caching mechanism. 
# The first COPY and RUN dotnet restore will only be re-executed if the OrderSystem.Worker.csproj file changes, which can speed up subsequent builds when only source code changes.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OrderSystem.Worker/OrderSystem.Worker.csproj", "OrderSystem.Worker/"]
COPY ["OrderSystem.Application/OrderSystem.Application.csproj", "OrderSystem.Application/"]
COPY ["OrderSystem.Infrastructure/OrderSystem.Infrastructure.csproj", "OrderSystem.Infrastructure/"]
COPY ["OrderSystem.Domain/OrderSystem.Domain.csproj", "OrderSystem.Domain/"]

RUN dotnet restore "OrderSystem.Worker/OrderSystem.Worker.csproj"
COPY . .
WORKDIR "/src/OrderSystem.Worker"
RUN dotnet publish -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderSystem.Worker.dll"]