# constants
$version = "0.14.0"
$driverName = "geckodriver.exe"
$zipNames = ("geckodriver-v$version-win32.zip", "geckodriver-v$version-win64.zip")
$baseUrl = "https://github.com/mozilla/geckodriver/releases/download/v$version/"

# move current folder to where contains this .ps1 script file.
$scriptDir = Split-Path $MyInvocation.MyCommand.Path
$workDir = Split-Path $scriptDir
pushd $workDir

$currentPath = Convert-Path "."

$zipNames | % {
    $zipName = $_
    $zipPath = Join-Path (Join-Path $currentPath "downloads") $zipName
    if ($zipName -like "*win32*"){
        $subDir = "win32"
    }
    else {
        $subDir = "win64"
    }
    $driverDir = Join-Path $currentPath "downloads\$subDir"
    $driverPath = Join-Path $driverDir $driverName

    # download driver .zip file if not exists.
    if (-not (Test-Path $zipPath)){
        (New-Object Net.WebClient).Downloadfile($baseUrl + $zipName, $zipPath)
        if (Test-Path $driverPath) { 
            del $driverPath 
        }
    }

    # Decompress .zip file to extract driver .exe file.
    if (-not (Test-Path $driverPath)) {
        $shell = New-Object -com Shell.Application
        $zipFile = $shell.NameSpace($zipPath)

        $zipFile.Items() | `
        where {(Split-Path $_.Path -Leaf) -eq $driverName} | `
        foreach {
            $cuurentDir = $shell.NameSpace($driverDir)
            $cuurentDir.copyhere($_.Path)
        }
        sleep(2)
    }
}
