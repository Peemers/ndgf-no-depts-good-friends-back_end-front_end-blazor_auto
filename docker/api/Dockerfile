FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ndgf.Api/ndgf.Api.csproj", "ndgf.Api/"]
COPY ["ndgf.Application/ndgf.Application.csproj", "ndgf.Application/"]
COPY ["ndgf.Domain/ndgf.Domain.csproj", "ndgf.Domain/"]
COPY ["ndgf.Infrastructure/ndgf.Infrastructure.csproj", "ndgf.Infrastructure/"]

RUN dotnet restore "ndgf.Api/ndgf.Api.csproj"

COPY . .

RUN dotnet publish "ndgf.Api/ndgf.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ndgf.Api.dll"]