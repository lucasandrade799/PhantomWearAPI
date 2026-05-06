# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PhantomWearAPI.csproj", "."]
RUN dotnet restore "PhantomWearAPI.csproj"
COPY . .
RUN dotnet publish "PhantomWearAPI.csproj" -c Release -o /app/publish

# Estágio Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Comando para rodar a API usando a porta que o Render fornecer
ENTRYPOINT ["dotnet", "PhantomWearAPI.dll"]
