#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY ./src .
RUN dotnet restore "Reservations.API/Reservations.API.csproj"

WORKDIR "/src/."
COPY ./src .
RUN dotnet publish "Reservations.API/Reservations.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "Reservations.API.dll"]
