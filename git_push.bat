@echo off
title Pushing to GitHub...
cd /d "E:\ass\assignment"

echo === Git status === > git_log.txt 2>&1
git status >> git_log.txt 2>&1

echo === Configuring git user === >> git_log.txt 2>&1
git config user.email "aiforassignment2@gmail.com" >> git_log.txt 2>&1
git config user.name "Dipesh Pokhrel" >> git_log.txt 2>&1

echo === Adding all files === >> git_log.txt 2>&1
git add -A >> git_log.txt 2>&1

echo === Committing === >> git_log.txt 2>&1
git commit -m "Add Dockerfile + production DB fix" >> git_log.txt 2>&1

echo === Pushing === >> git_log.txt 2>&1
git push origin main --force >> git_log.txt 2>&1

echo === Done === >> git_log.txt 2>&1
echo Finished! Check git_log.txt for details.
timeout /t 5 /nobreak >nul
