FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ndgf.Web/ndgf.Web/ndgf.Web.csproj"
RUN dotnet publish "ndgf.Web/ndgf.Web/ndgf.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ndgf.Web.dll"]