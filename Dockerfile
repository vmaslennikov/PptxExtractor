ARG NODE_IMAGE=node:12.14
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PptxExtractor.csproj", ""]
RUN dotnet restore "./PptxExtractor.csproj"
COPY . .
WORKDIR "/src"

FROM ${NODE_IMAGE} as node-build
WORKDIR /src
COPY ./ClientApp .
RUN npm install
RUN npm run build -- --prod

FROM build AS publish
RUN dotnet publish "PptxExtractor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node-build /src/dist ./ClientApp/dist
ENTRYPOINT ["dotnet", "PptxExtractor.dll"]