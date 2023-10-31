FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EmailMQReceiver/Calori.EmailRabbitMQReceiver.csproj", "EmailMQReceiver/"]
RUN dotnet restore "EmailMQReceiver/Calori.EmailRabbitMQReceiver.csproj"
COPY . .
WORKDIR "/src/EmailMQReceiver"
RUN dotnet build "Calori.EmailRabbitMQReceiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calori.EmailRabbitMQReceiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calori.EmailRabbitMQReceiver.dll"]
