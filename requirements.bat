@echo off

dotnet build lc-hax -C Release
dotnet publish submodules/SharpMonoInjectorCore

pause
