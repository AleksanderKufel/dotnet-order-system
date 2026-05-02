# build stage
# Two steps of build COPY and RUN are used to leverage Docker's layer caching mechanism. 
# The first COPY and RUN dotnet restore will only be re-executed if the OrderSystem.API.csproj file changes, which can speed up subsequent builds when only source code changes.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OrderSystem.API/OrderSystem.API.csproj", "OrderSystem.API/"]
COPY ["OrderSystem.Infrastructure/OrderSystem.Infrastructure.csproj", "OrderSystem.Infrastructure/"]
COPY ["OrderSystem.Application/OrderSystem.Application.csproj", "OrderSystem.Application/"]
COPY ["OrderSystem.Domain/OrderSystem.Domain.csproj", "OrderSystem.Domain/"]

RUN dotnet restore "OrderSystem.API/OrderSystem.API.csproj"
COPY . .
WORKDIR "/src/OrderSystem.API"
RUN dotnet publish -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderSystem.API.dll"]