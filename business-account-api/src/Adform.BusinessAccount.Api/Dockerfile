#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Adform.BusinessAccount.Api/Adform.BusinessAccount.Api.csproj", "Adform.BusinessAccount.Api/"]
RUN dotnet restore "Adform.BusinessAccount.Api/Adform.BusinessAccount.Api.csproj"
COPY . .
WORKDIR "/src/Adform.BusinessAccount.Api"
RUN dotnet build "Adform.BusinessAccount.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Adform.BusinessAccount.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Adform.BusinessAccount.Api.dll"]