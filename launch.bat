@echo off

:begin
set project_name=lc-hax
dotnet build %project_name%
start /wait /b ./submodules/SharpMonoInjectorCore/dist/SharpMonoInjector.exe inject -p "Lethal Company" -a bin/%project_name%.dll -n Hax -c Loader -m Load

pause
goto begin
