FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PingMeChat.CMS.AdminPage/PingMeChat.CMS.AdminPage.csproj", "PingMeChat.CMS.AdminPage/"]
COPY ["PingMeChat.CMS.Application/PingMeChat.CMS.Application.csproj", "PingMeChat.CMS.Application/"]
COPY ["PingMeChat.CMS.Entities/PingMeChat.CMS.Entities.csproj", "PingMeChat.CMS.Entities/"]
COPY ["PingMeChat.CMS.EntityFrameworkCore/PingMeChat.CMS.EntityFrameworkCore.csproj", "PingMeChat.CMS.EntityFrameworkCore/"]
COPY ["PingMeChat.Shared/PingMeChat.Shared.csproj", "PingMeChat.Shared/"]
COPY ["PingMeChat.CMS.Api/PingMeChat.CMS.Api.csproj", "PingMeChat.CMS.Api/"]
RUN dotnet restore "PingMeChat.CMS.Api/PingMeChat.CMS.Api.csproj"
COPY . .
WORKDIR "/src/PingMeChat.CMS.Api"
RUN dotnet build "PingMeChat.CMS.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PingMeChat.CMS.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/logs
ENTRYPOINT ["dotnet", "PingMeChat.CMS.Api.dll"]