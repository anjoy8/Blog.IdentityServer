#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Blog.IdentityServer/Blog.IdentityServer.csproj", "Blog.IdentityServer/"]
RUN dotnet restore "Blog.IdentityServer/Blog.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Blog.IdentityServer"
RUN dotnet build "Blog.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blog.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.IdentityServer.dll"]