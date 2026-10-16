@echo off

:begin
dotnet publish launcher\launcher.csproj -restoreProperty:RestoreLockedMode=true
.\launcher.exe

goto begin
