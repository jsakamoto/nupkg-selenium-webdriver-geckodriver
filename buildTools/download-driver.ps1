# constants
$version = "0.32.1"
$downloadUrlBase = "https://github.com/mozilla/geckodriver/releases/download"

$drivers = @(
    [ordered]@{
        platform    = "win32";
        folder      = "win32";
        fileName    = "geckodriver.exe";
        archiveType = "zip";
    }
    ,
    [ordered]@{
        platform    = "win64";
        folder      = "win64";
        fileName    = "geckodriver.exe";
        archiveType = "zip";
    }
    ,
    [ordered]@{
        platform    = "macos";
        folder      = "mac64";
        fileName    = "geckodriver";
        archiveType = "tar.gz";
    }
    ,
    [ordered]@{
        platform    = "macos-aarch64";
        folder      = "mac64arm";
        fileName    = "geckodriver";
        archiveType = "tar.gz";
    }
    ,
    [ordered]@{
        platform    = "linux64";
        folder      = "linux64";
        fileName    = "geckodriver";
        archiveType = "tar.gz";
    }
)

# move current folder to where contains this .ps1 script file.
$scriptDir = Split-Path $MyInvocation.MyCommand.Path
Push-Location $scriptDir
Set-Location ..
$currentPath = Convert-Path "."
$downloadsBaseDir = Join-Path $currentPath "downloads"

# setup build tools command path
$unzip = "$currentPath\buildTools\unzip.exe"
$gzip = "$currentPath\buildTools\gzip.exe"
$tar = "$currentPath\buildTools\tar.exe"

# enable TLS1.2 for connection to GitHub.
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [System.Net.SecurityProtocolType]::Tls12

# process each drivers
$drivers | ForEach-Object {
    $driver = $_
    $driverName = $driver.fileName
    $platform = $driver.platform
    $folder = $driver.folder
    $archiveType = $driver.archiveType

    $downloadDir = Join-Path $downloadsBaseDir $folder
    $driverPath = Join-Path $downloadDir $driverName

    # download driver .zip/.tar.gz file if not exists.
    $archiveName = "geckodriver-v$version-$platform.$archiveType"
    $archivePath = Join-Path $downloadDir $archiveName
    if (-not (Test-Path $downloadDir)) { mkdir $downloadDir > $null }
    if (-not (Test-Path $archivePath)) {
        $downloadUrl = "$downloadUrlBase/v$version/$archiveName"
        Write-Host $downloadUrl
        Invoke-WebRequest -Uri $downloadUrl -OutFile $archivePath
        if (Test-Path $driverPath) {
            Remove-Item $driverPath 
        }
    }

    # Decompress .zip/.tar.gz file to extract driver file.
    if (-not (Test-Path $driverPath)) {
        Set-Location $downloadDir
        if ($archiveType -eq "zip") {
            & $unzip -q $archiveName
        }
        if ($archiveType -eq "tar.gz") {
            & $gzip -kdf $archiveName
            & $tar -xf ((Get-ChildItem $archiveName).BaseName)
        }
        Set-Location $currentPath
    }
}
