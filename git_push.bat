@echo off
title Pushing to GitHub...
cd /d "E:\ass\assignment"

echo === Git status === > git_log.txt 2>&1
git status >> git_log.txt 2>&1

echo === Configuring git user === >> git_log.txt 2>&1
git config user.email "aiforassignment2@gmail.com" >> git_log.txt 2>&1
git config user.name "Dipesh Pokhrel" >> git_log.txt 2>&1

echo === Adding finalevent remote (if not already exists) === >> git_log.txt 2>&1
git remote add finalevent https://github.com/Dipesh12-pokhrel/finalevent.git >> git_log.txt 2>&1

echo === Adding all files === >> git_log.txt 2>&1
git add -A >> git_log.txt 2>&1

echo === Committing === >> git_log.txt 2>&1
git commit -m "Railway deployment: root Dockerfile + railway.toml + landing page" >> git_log.txt 2>&1

echo === Pushing to final-assignment- === >> git_log.txt 2>&1
git push origin main --force >> git_log.txt 2>&1

echo === Pushing to finalevent === >> git_log.txt 2>&1
git push finalevent main --force >> git_log.txt 2>&1

echo === Done === >> git_log.txt 2>&1
echo Finished! Check git_log.txt for details.
timeout /t 5 /nobreak >nul
