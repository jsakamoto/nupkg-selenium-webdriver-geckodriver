@echo off
pushd %~dp0

echo Downloading %fname%...
powershell -noprof -exec unrestricted -c ".\buildTools\download-driver.ps1"
echo.
:SKIP_DOWNLOAD

echo Packaging...
.\buildTools\NuGet.exe pack .\src\Selenium.WebDriver.GeckoDriver.Win32.nuspec -Out .\dist
.\buildTools\NuGet.exe pack .\src\Selenium.WebDriver.GeckoDriver.Win64.nuspec -Out .\dist
