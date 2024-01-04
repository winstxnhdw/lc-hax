@echo off

git submodule update --init
dotnet build lc-hax -c Release
dotnet publish submodules/SharpMonoInjectorCore

pause
