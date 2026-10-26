@echo off

:begin
dotnet publish launcher -restoreProperty:RestoreLockedMode=true
.\launcher.exe

goto begin
