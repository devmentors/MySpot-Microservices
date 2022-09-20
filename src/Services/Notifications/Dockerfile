FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
# RUN dotnet publish MySpot.Services.Notifications.Api -c release -o out
RUN dotnet publish src/Services/Notifications/MySpot.Services.Notifications.Api -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet MySpot.Services.Notifications.Api.dll