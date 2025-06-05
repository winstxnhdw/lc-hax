@echo off

set project_name=lc-hax

:begin
dotnet build %project_name% -c Release -restoreProperty:RestoreLockedMode=true
start /wait /b ./submodules/SharpMonoInjectorCore/dist/SharpMonoInjector.exe inject -p "Lethal Company" -a bin/%project_name%.dll -n Hax -c Loader -m Load

pause
goto begin
