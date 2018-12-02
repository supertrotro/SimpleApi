FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Simple.Api/Simple.Api.csproj", "Simple.Api/"]
RUN dotnet restore "Simple.Api/Simple.Api.csproj"
COPY . .
WORKDIR "/src/Simple.Api"
RUN dotnet build "Simple.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Simple.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Simple.Api.dll"]