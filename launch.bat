@echo off

where /q git || winget install Git.Git
where /q dotnet || winget install Microsoft.DotNet.SDK.10
where /q git || echo Please run launch.bat again && exit
where /q dotnet || echo Please run launch.bat again && exit

if not exist ".git" git init --initial-branch=main

git remote get-url origin || git remote add origin https://github.com/winstxnhdw/lc-hax.git
git pull --rebase --autostash origin main
winget upgrade Microsoft.DotNet.SDK.10

:begin
dotnet publish launcher -restoreProperty:RestoreLockedMode=true || exit
.\launcher.exe

goto begin
