# PowerLauncher Build Script
# Publishes the app and creates the installer

Write-Host "=== PowerLauncher Build ===" -ForegroundColor Cyan

# Step 1: Publish
Write-Host "`n[1/3] Publishing application..." -ForegroundColor Yellow
dotnet publish MinecraftLauncher/MinecraftLauncher.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o MinecraftLauncher/bin/Release/net8.0-windows/publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "[2/3] Build successful!" -ForegroundColor Green

# Step 2: Create installer output dir
New-Item -ItemType Directory -Force -Path "installer/output" | Out-Null

# Step 3: Run Inno Setup (if installed)
$innoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (Test-Path $innoPath) {
    Write-Host "[3/3] Creating installer..." -ForegroundColor Yellow
    & $innoPath "installer/setup.iss"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nInstaller created at: installer/output/" -ForegroundColor Green
    }
} else {
    Write-Host "[3/3] Inno Setup not found at: $innoPath" -ForegroundColor Yellow
    Write-Host "  Install Inno Setup 6 from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host "  Then run: ISCC.exe installer/setup.iss" -ForegroundColor Yellow
}

Write-Host "`n=== Done ===" -ForegroundColor Cyan
