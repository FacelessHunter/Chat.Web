FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY /dist/ .
ENTRYPOINT ["dotnet", "Chat.Web.dll"]