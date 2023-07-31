FROM alpine:latest AS build
RUN apk add dotnet6-sdk
RUN mkdir /src
WORKDIR /src
COPY ["LPRMock.csproj", "."]
RUN dotnet restore "./LPRMock.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LPRMock.csproj" -c Release -o /app/build
RUN dotnet publish "LPRMock.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM alpine:latest AS final
RUN apk update && \
    apk upgrade && \
    apk add aspnetcore6-runtime &&\
    apk cache clean
RUN mkdir /app &&\
    adduser -D appuser && \
    chown -R appuser /app
WORKDIR /app
COPY --from=build --chown=appuser /app/publish .
EXPOSE 4200 515

USER appuser
ENTRYPOINT ["dotnet", "LPRMock.dll"]