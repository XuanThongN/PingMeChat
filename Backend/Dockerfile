FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BIA.CMS.AdminPage/BIA.CMS.AdminPage.csproj", "BIA.CMS.AdminPage/"]
COPY ["BIA.CMS.Application/BIA.CMS.Application.csproj", "BIA.CMS.Application/"]
COPY ["BIA.CMS.Entities/BIA.CMS.Entities.csproj", "BIA.CMS.Entities/"]
COPY ["BIA.CMS.EntityFrameworkCore/BIA.CMS.EntityFrameworkCore.csproj", "BIA.CMS.EntityFrameworkCore/"]
COPY ["BIA.Shared/BIA.Shared.csproj", "BIA.Shared/"]
RUN dotnet restore "BIA.CMS.AdminPage/BIA.CMS.AdminPage.csproj"
COPY . .
WORKDIR "/src/BIA.CMS.AdminPage"
RUN dotnet build "BIA.CMS.AdminPage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BIA.CMS.AdminPage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/logs
ENTRYPOINT ["dotnet", "BIA.CMS.AdminPage.dll"]