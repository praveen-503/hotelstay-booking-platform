FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY HotelStay.sln ./
COPY HotelStay.Api/HotelStay.Api.csproj HotelStay.Api/
COPY HotelStay.Tests/HotelStay.Tests.csproj HotelStay.Tests/

RUN dotnet restore HotelStay.sln

COPY . .
RUN dotnet publish HotelStay.Api/HotelStay.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

CMD ["sh", "-c", "dotnet HotelStay.Api.dll --urls http://0.0.0.0:${PORT:-8080}"]
