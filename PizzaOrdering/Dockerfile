FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /PizzaOrdering

# Copy everything
COPY . ./
# Restore asdistinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o app 

# Build runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /PizzaOrdering
COPY --from=build-env /PizzaOrdering/app .

ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT [ "dotnet", "PizzaOrdering.dll" ]
