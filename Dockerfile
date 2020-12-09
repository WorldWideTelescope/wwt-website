# Build the app
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out wwt-website-net5.slnf

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
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
