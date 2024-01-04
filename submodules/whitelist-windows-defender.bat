@echo off
echo Windows Defender hates SharpMonoInjector, attempting to whitelist...

net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [91mERROR: This script must run as administrator.[0m
    pause
    exit /b -1
)

for %%p in ("%~dp0" "%TEMP%\.net\SharpMonoInjector") do (
    powershell -ExecutionPolicy Bypass -Command "Add-MpPreference -ExclusionPath "%%p"" || goto :failed
)

echo [92mSuccess![0m
pause
exit /b 0

:failed
echo [91mERROR: Failed to whitelist.[0m
pause
exit /b 1
