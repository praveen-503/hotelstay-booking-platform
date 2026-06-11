#!/usr/bin/env sh
dotnet HotelStay.Api.dll --urls "http://0.0.0.0:${PORT:-8080}"
