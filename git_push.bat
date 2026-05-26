@echo off
title Pushing to GitHub...
cd /d "E:\ass\assignment"

echo === Initializing git repository ===
git init
git branch -M main

echo === Configuring git user ===
git config user.email "aiforassignment2@gmail.com"
git config user.name "Dipesh Pokhrel"

echo === Adding all files ===
git add .

echo === Committing ===
git commit -m "Complete Event Management System - Blazor Server .NET 8"

echo === Setting remote ===
git remote remove origin 2>nul
git remote add origin https://github.com/Dipesh12-pokhrel/final-assignment-.git

echo === Pushing to GitHub (may ask for login) ===
git push -u origin main --force

echo.
echo === Done! ===
echo Check https://github.com/Dipesh12-pokhrel/final-assignment-
pause
