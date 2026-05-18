
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Boi.Net/Boi.Net.csproj", "Boi.Net/"]
RUN dotnet restore "Boi.Net/Boi.Net.csproj"

COPY . .

RUN dotnet publish "Boi.Net/Boi.Net.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

LABEL maintainer="Boi.Net Development Team"
LABEL description="Boi.Net - Bookstore E-commerce Platform API"

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "Boi.Net.dll"]