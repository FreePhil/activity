FROM microsoft/dotnet:2.2-sdk AS build

WORKDIR /src/ActivityService
COPY ActivityService/ActivityService.csproj .
RUN dotnet restore ./ActivityService.csproj
COPY ActivityService/ .

WORKDIR /src/ActivityService
RUN dotnet build ActivityService.csproj -c Release

FROM build AS publish
RUN dotnet publish ActivityService.csproj -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ActivityService.dll"]
