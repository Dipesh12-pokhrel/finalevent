@echo off
title Restarting EventHub...
echo Stopping any running dotnet processes...
taskkill /F /IM dotnet.exe /T 2>nul
timeout /t 2 /nobreak >nul
echo Starting EventHub with updated code...
cd /d "E:\ass\assignment\EventManagement"
start "EventHub - Blazor Server" cmd /k "dotnet run && pause"
echo Done! App starting at http://localhost:5000
timeout /t 3 /nobreak >nul
