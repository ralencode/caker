FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS server-build
WORKDIR /src

COPY Caker.* ./
RUN dotnet restore

COPY . .

RUN dotnet publish "Caker.csproj" -c Release -o /app/publish /p:DebugType=portable


FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

COPY --from=server-build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:40000
EXPOSE 40000 8080

ENTRYPOINT ["dotnet", "Caker.dll"]