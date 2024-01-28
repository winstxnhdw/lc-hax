@echo off

git pull --rebase --autostash
git checkout --theirs .
git add .
call launch-dev.bat
