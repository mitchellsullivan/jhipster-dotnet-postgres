FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Plainly.Data/Plainly.Data.csproj", "Plainly.Data/"]
RUN dotnet restore "src/Plainly.Data/Plainly.Data.csproj"
COPY . .
WORKDIR "/src/Plainly.Data"
RUN dotnet build "Plainly.Data.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Plainly.Data.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Plainly.Data.dll"]
