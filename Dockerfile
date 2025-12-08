###############################syntax=docker
# Build stage
###############################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["DotPic.csproj", "./"]


RUN --mount=type=cache,target=/root/.nuget \
 dotnet restore "./DotPic.csproj"


COPY . .
RUN dotnet build "./DotPic.csproj" -c Release -o /app/build


###############################
# Publish stage
###############################
FROM build AS publish
RUN dotnet publish "./DotPic.csproj" -c Release -o /app/publish /p:UseAppHost=false


###############################
# Final runtime stage
###############################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app


COPY --from=publish /app/publish .

# Ensure DataProtection keys directory exists and is owned by the `app` user
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys \
	&& chown -R app:app /home/app/.aspnet

USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "DotPic.dll"]
