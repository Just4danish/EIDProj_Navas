FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# copy csproj and restore as distinct layers

WORKDIR /src
COPY *.sln .
COPY ["EIDReaderWebWrapper/*.csproj", "EIDReaderWebWrapper/"]
COPY ["EIDReaderLib/*.csproj", "EIDReaderLib/"]
RUN dotnet restore
RUN nuget restore

WORKDIR /src

# copy and build everything else
COPY . .
WORKDIR /src
#RUN dotnet publish -c Release -o out --no-restore
RUN msbuild /p:Configuration=Release -r:False

FROM mcr.microsoft.com/dotnet/framework/runtime:4.8 AS runtime
WORKDIR /app
#COPY --from=build /app/out ./
COPY --from=build /src/EIDReaderWebWrapper/bin/Release ./
ENTRYPOINT ["EIDReaderWebWrapper.exe"]


## Copied from core web app
#FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
#WORKDIR /src
#COPY ["EIDReaderWebWrapperCore/EIDReaderWebWrapperCore.csproj", "EIDReaderWebWrapperCore/"]
#RUN dotnet restore "EIDReaderWebWrapperCore/EIDReaderWebWrapperCore.csproj"
#COPY . .
#WORKDIR "/src/EIDReaderWebWrapperCore"
#RUN dotnet build "EIDReaderWebWrapperCore.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "EIDReaderWebWrapperCore.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "EIDReaderWebWrapperCore.dll"]
