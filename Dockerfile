FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore
#build and publish a release
RUN dotnet publish -c Release -o out


# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8085
ENTRYPOINT [ "dotnet","EmployeeManagement.dll" ]


