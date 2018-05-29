FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ShoppingCart/*.csproj ./ShoppingCart/
RUN dotnet restore

# Copy everything else and build
COPY ShoppingCart/. ./ShoppingCart/
WORKDIR /app/ShoppingCart
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
COPY --from=build /app/ShoppingCart/out ./
ENTRYPOINT ["dotnet", "ShoppingCart.dll"]
