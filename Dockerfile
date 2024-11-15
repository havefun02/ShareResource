FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . /src
RUN dotnet restore "ShareResource.csproj"

RUN dotnet build "ShareResource.csproj" -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "ShareResource.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ShareResource.exe"]


