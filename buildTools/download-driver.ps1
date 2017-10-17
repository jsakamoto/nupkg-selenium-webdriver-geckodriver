# constants
$version = "0.19.0"
$downloadUrlBase = "https://github.com/mozilla/geckodriver/releases/download"

$drivers = @(
    [ordered]@{
        platform    = "win32";
        fileName    = "geckodriver.exe";
        archiveType = "zip";
    }
    ,
    [ordered]@{
        platform    = "win64";
        fileName    = "geckodriver.exe";
        archiveType = "zip";
    }
    ,
    [ordered]@{
        platform    = "macos";
        fileName    = "geckodriver";
        archiveType = "tar.gz";
    }
    ,
    [ordered]@{
        platform    = "linux64";
        fileName    = "geckodriver";
        archiveType = "tar.gz";
    }
)

# move current folder to where contains this .ps1 script file.
$scriptDir = Split-Path $MyInvocation.MyCommand.Path
pushd $scriptDir
cd ..
$currentPath = Convert-Path "."
$downloadsBaseDir = Join-Path $currentPath "downloads"

# setup build tools command path
$unzip = "$currentPath\buildTools\unzip.exe"
$gzip = "$currentPath\buildTools\gzip.exe"
$tar = "$currentPath\buildTools\tar.exe"

# process each drivers
$drivers | % {
    $driver = $_
    $driverName = $driver.fileName
    $platform = $driver.platform
    $archiveType = $driver.archiveType

    $downloadDir = Join-Path $downloadsBaseDir $platform
    $driverPath = Join-Path $downloadDir $driverName

    # download driver .zip/.tar.gz file if not exists.
    $zipName = "geckodriver-v$version-$platform.$archiveType"
    $zipPath = Join-Path $downloadDir $zipName
    if (-not (Test-Path $zipPath)) {
        $downloadUrl = "$downloadUrlBase/v$version/$zipName"
        (New-Object Net.WebClient).Downloadfile($downloadurl, $zipPath)
        if (Test-Path $driverPath) {
            del $driverPath 
        }
    }

    # Decompress .zip/.tar.gz file to extract driver file.
    if (-not (Test-Path $driverPath)) {
        cd $downloadDir
        if ($archiveType -eq "zip") {
            & $unzip -q $zipName
        }
        if ($archiveType -eq "tar.gz") {
            & $gzip -kdf $zipName
            & $tar -xf ((ls $zipName).BaseName)
        }
        cd $currentPath
    }
}
