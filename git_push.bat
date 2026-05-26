@echo off
title Pushing to GitHub...
cd /d "E:\ass\assignment"

echo === Configuring git user ===
git config user.email "aiforassignment2@gmail.com"
git config user.name "Dipesh Pokhrel"

echo === Adding all files ===
git add .

echo === Committing ===
git commit -m "Add Dockerfile + production DB fix - Event Management System"

echo === Pushing to GitHub ===
git push origin main --force

echo.
echo === Done! ===
echo Check https://github.com/Dipesh12-pokhrel/final-assignment-
pause
