FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /PizzaCatalog

# Copy everything
COPY . ./
# Restore asdistinct layers
RUN dotnet restore
# Build and publish a release
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \ 
	dotnet publish -c Release --use-current-runtime --self-contained false -o app

# Build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /PizzaCatalog
COPY --from=build-env /PizzaCatalog/app .

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT [ "dotnet", "PizzaCatalog.dll" ]
