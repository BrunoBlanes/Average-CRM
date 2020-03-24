# Predefined Variables
$adSdkUrl = "https://software-download.microsoft.com/download/sg/Windows_InsiderPreview_SDK_en-us_19041_1.iso"
$adSdkPath = Join-Path $env:TEMP "Windows_InsiderPreview_SDK_en-us_19041_1.iso"

# Download the files to local temp folder
Write-Output "Downloading Windows Insider Preview SDK 19041"
$progressPreference = 'silentlyContinue'
Invoke-WebRequest -Uri $adSdkUrl -OutFile $adSdkPath
$progressPreference = 'Continue'

# Mount the SDKs
Write-Output "Mounting Windows Insider Preview SDK 19041"
$mountResult =  Mount-DiskImage -ImagePath $adSdkPath -PassThru
$driveLetter = ($mountResult | Get-Volume).DriveLetter
Write-Output "Mounted Windows Insider Preview SDK 19041 at $driveLetter`:\"

# Install the SDKs (use the "qn" flag to install silently)
$adSdkPath =  $driveLetter + ':\WinSDKSetup.exe'
Write-Output "Installing Windows Insider Preview SDK 19041"	
Start-Process $adSdkPath -ArgumentList "/q" -Wait
Write-Output "Windows Insider Preview SDK 19041 Successfully Installed"