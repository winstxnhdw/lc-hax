@echo off

git submodule update --init || pause && exit /b
dotnet build lc-hax -c Release
dotnet publish submodules/SharpMonoInjectorCore

pause
