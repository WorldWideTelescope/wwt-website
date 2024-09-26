# Build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out ./src/WWT.Web/WWT.Web.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
RUN apt-get update \
  && DEBIAN_FRONTEND=noninteractive apt-get install -y libgdiplus \
  && apt-get clean \
  && rm -rf /var/lib/apt/lists/*
COPY --from=build-env /app/out .

ENV ASPNETCORE_ENVIRONMENT Production
# To avoid "Could not find a suitable shadow copy folder." in App Insights Snapshot Debugger:
ENV TEMP /tmp

ENTRYPOINT ["dotnet", "WWT.Web.dll"]
