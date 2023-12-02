@echo off

set project_name=lc-hax
dotnet build %project_name%
dotnet publish submodules/SharpMonoInjectorCore
