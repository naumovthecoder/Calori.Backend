FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Calori.WebApi/Calori.WebApi.csproj", "Calori.WebApi/"]
COPY ["Calori.Persistence/Calori.Persistence.csproj", "Calori.Persistence/"]
COPY ["Calori.Domain/Calori.Domain.csproj", "Calori.Domain/"]
COPY ["Calori.EmailService/Calori.EmailService.csproj", "Calori.EmailService/"]
COPY ["Calori.Application/Calori.Application.csproj", "Calori.Application/"]
RUN dotnet restore "Calori.WebApi/Calori.WebApi.csproj"
COPY . .
WORKDIR "/src/Calori.WebApi"
RUN dotnet build "Calori.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calori.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calori.WebApi.dll"]
