#!/usr/bin/env bash
# HotelStay Booking Platform Launcher for Unix/macOS/Git Bash

echo "============================================="
echo "   HotelStay Booking Platform Launcher"
echo "============================================="

# 1. Install frontend dependencies if needed
if [ ! -d "hotelstay-ui/node_modules" ]; then
    echo "Installing frontend dependencies..."
    cd hotelstay-ui && npm install && cd ..
fi

# 2. Run backend and frontend concurrently
echo "Starting backend and frontend concurrently..."
npm install
npm run start
