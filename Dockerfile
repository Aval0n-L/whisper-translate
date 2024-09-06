FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src

COPY src/. .

# Publish the application with the following configurations:
# - Configuration: Release mode for optimized production code
# - Output: Place the published output in the /app/publish directory
# - Runtime: Target the linux-x64 platform
# - Self-contained: false, meaning it relies on the system's .NET runtime rather than including it
# - ReadyToRun: Enable ReadyToRun compilation to improve startup performance
RUN dotnet publish \
    --configuration Release \
    --output /app/publish \
    --runtime linux-x64 \
    --self-contained false \
    -p:PublishReadyToRun=true \
    WhisperTranslate/WhisperTranslate.csproj


FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhisperTranslate.dll"]