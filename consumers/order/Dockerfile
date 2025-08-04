FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the .csproj files first
# (improves caching - Docker builds each instruction as a separate cacheable layer, so this
# allows the dotnet restore layer to potentially be reused as the csproj is unlikely to change often)
COPY order/order.csproj ./order/
COPY shared/shared.csproj ./shared/

# Copy full source
COPY order/. ./order/
COPY shared/. ./shared/

# Restore, build, publish
WORKDIR /src/order
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "order.dll"]
