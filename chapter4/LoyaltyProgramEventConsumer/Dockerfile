FROM microsoft/dotnet:2.1-runtime-alpine AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY . .
#WORKDIR /src/LoyaltyProgramEventConsumer
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LoyaltyProgramEventConsumer.dll"]