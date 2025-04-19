# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento/Vidsnap.Worker.AtualizaStatusProcessamento.csproj", "Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento/"]
COPY ["Core/Vidsnap.Application/Vidsnap.Application.csproj", "Core/Vidsnap.Application/"]
COPY ["Core/Vidsnap.Domain/Vidsnap.Domain.csproj", "Core/Vidsnap.Domain/"]
COPY ["Driven/Vidsnap.DataBase/Vidsnap.DataBase.csproj", "Driven/Vidsnap.DataBase/"]
COPY ["Driven/Vidsnap.SQS/Vidsnap.SQS.csproj", "Driven/Vidsnap.SQS/"]
RUN dotnet restore "./Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento/Vidsnap.Worker.AtualizaStatusProcessamento.csproj"

COPY ["Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento/", "Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento/"]
COPY ["Core/Vidsnap.Application/", "Core/Vidsnap.Application/"]
COPY ["Core/Vidsnap.Domain/", "Core/Vidsnap.Domain/"]
COPY ["Driven/Vidsnap.DataBase/", "Driven/Vidsnap.DataBase/"]
COPY ["Driven/Vidsnap.SQS/", "Driven/Vidsnap.SQS/"]

WORKDIR "/src/Driving/Vidsnap.Worker.AtualizaStatus/Vidsnap.Worker.AtualizaStatusProcessamento"
RUN dotnet build "./Vidsnap.Worker.AtualizaStatusProcessamento.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vidsnap.Worker.AtualizaStatusProcessamento.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final

# Cria um usuário com ID 1000
RUN useradd --create-home --uid 1000 appuser

WORKDIR /app
COPY --from=publish /app/publish .

# Define o usuário padrão
RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Vidsnap.Worker.AtualizaStatusProcessamento.dll"]