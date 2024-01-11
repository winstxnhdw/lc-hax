@echo off

git submodule update --init || pause && exit /b

echo "Building SharpMonoInjectorCore! Please be patient as this may take a few minutes!"
dotnet publish submodules/SharpMonoInjectorCore

pause
