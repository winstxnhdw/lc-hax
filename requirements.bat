@echo off

dotnet build lc-hax -c Release
dotnet publish submodules/SharpMonoInjectorCore

pause
