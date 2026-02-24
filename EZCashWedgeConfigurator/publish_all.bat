@echo off
cd /d "%~dp0"

:: Set a variable to prevent the infinite loop
set IS_PUBLISHING=true

echo Cleaning old publish files...
if exist ".\Publish" rd /s /q ".\Publish"

echo Publishing x86 version...
dotnet publish "EZCashWedgeConfigurator.csproj" -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -p:DebugSymbols=false -o ".\Publish\x86"

echo Done!