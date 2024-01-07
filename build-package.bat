@echo off
pushd %~dp0

dotnet pack .\buildTools\Selenium.WebDriver.GeckoDriver.csproj

echo Complete!
echo.