FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY OpenAAP/OpenAAP.csproj /src/
RUN dotnet restore
COPY OpenAAP/* /src/
RUN dotnet publish -c Release -o /app


FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
EXPOSE 80
WORKDIR /app
COPY --from=build /app /app
ENTRYPOINT ["dotnet", "OpenAAP.dll"]
