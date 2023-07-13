#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM docker.artifactory.adform.com/dotnet/sdk:6.0.100-bullseye-slim as publish
WORKDIR /src
COPY ./src .
COPY ./NuGet.config .
WORKDIR /src/Adform.BusinessAccount.Api
RUN dotnet restore Adform.BusinessAccount.Api.csproj && dotnet publish Adform.BusinessAccount.Api.csproj -c Release -o /app

FROM docker.artifactory.adform.com/dotnet/aspnet:6.0.1-bullseye-slim
WORKDIR /app
COPY --from=publish /app .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "Adform.BusinessAccount.Api.dll"]