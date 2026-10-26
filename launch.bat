@echo off

if not exist ".git" git init --initial-branch=main

git remote get-url origin || git remote add origin https://github.com/winstxnhdw/lc-hax.git
git pull --rebase --autostash origin main
dotnet publish launcher -restoreProperty:RestoreLockedMode=true
.\launcher.exe
